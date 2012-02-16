﻿namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy;
    using Nancy.Cookies;
    using Xunit;
    using Xunit.Extensions;

    public class RequestHeadersFixture
    {
        [Fact]
        public void Should_return_all_header_values_when_values_are_retrieved()
        {
            // Given
            var rawHeaders =
                new Dictionary<string, IEnumerable<string>>
                {
                    {"accept", new[] {"text/plain", "text/html"}},
                    {"charset", new[] {"utf-8"}}
                };

            var headers = new RequestHeaders(rawHeaders);

            // When
            var values = headers.Values.ToList();

            // Then
            values.Count().ShouldEqual(2);
            values.First().Count().ShouldEqual(2);
            values.First().First().ShouldEqual("text/plain");
            values.First().Last().ShouldEqual("text/html");
            values.Last().First().ShouldEqual("utf-8");
        }

        [Fact]
        public void Should_return_all_header_names_when_keys_are_retrieved()
        {
            // Given
            var rawHeaders = 
                new Dictionary<string, IEnumerable<string>>
                {
                    {"accept", null},
                    {"charset", null}
                };

            var headers = new RequestHeaders(rawHeaders);

            // When
            var keys = headers.Keys.ToList();

            // Then
            keys.Count().ShouldEqual(2);
            keys.First().ShouldEqual("accept");
            keys.Last().ShouldEqual("charset");
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);
            
            // Then
            headers.Accept.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_all_accept_headers_when_multiple_are_available()
        {
            // Given
            var rawHeaders = 
                new Dictionary<string, IEnumerable<string>>
                {
                    { "Accept", new[] { "text/plain", "text/ninja" } }
                };

            // When
            var headers = new RequestHeaders(rawHeaders).Accept.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].Item1.ShouldEqual("text/plain");
            headers[1].Item1.ShouldEqual("text/ninja");
        }

        [Fact]
        public void Should_parse_accept_header_values_when_containing_multiple_values()
        {
            // Given
            var values = new[] { "text/plain, text/ninja" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).Accept.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].Item1.ShouldEqual("text/plain");
            headers[1].Item1.ShouldEqual("text/ninja");
        }

        [Fact]
        public void Should_parse_accept_header_value_quality_when_available()
        {
            // Given
            var values = new[] { "text/plain;q=0.3" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).Accept.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("text/plain");
            headers[0].Item2.ShouldEqual(0.3m);
        }

        [Fact]
        public void Should_default_accept_header_values_to_quality_one_if_not_explicitly_defined()
        {
            // Given
            var values = new[] { "text/plain" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).Accept.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("text/plain");
            headers[0].Item2.ShouldEqual(1m);
        }

        [Fact]
        public void Should_ignore_accept_header_values_with_invalid_quality()
        {
            // Given
            var values = new[] { "text/plain, text/ninja;q=a" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).Accept.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("text/plain");
        }

        [Fact]
        public void Should_sort_accept_header_values_decending_based_on_quality()
        {
            // Given
            var values = new[] { "text/plain;q=0.3, text/ninja, text/html;q=0.7" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).Accept.ToList();

            // Then
            headers.Count.ShouldEqual(3);
            headers[0].Item1.ShouldEqual("text/ninja");
            headers[1].Item1.ShouldEqual("text/html");
            headers[2].Item1.ShouldEqual("text/plain");
        }

        [Theory]
        [InlineData("accept")]
        [InlineData("AcCepT")]
        public void Should_ignore_case_of_accept_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "text/plain", "text/ninja" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders).Accept.ToList();

            // Then
            headers[0].Item1.ShouldEqual("text/plain");
            headers[1].Item1.ShouldEqual("text/ninja");
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_charset_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.AcceptCharset.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_accept_charset_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "utf-8", "iso-8859-5" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Charset", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptCharset.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].Item1.ShouldEqual("utf-8");
            headers[1].Item1.ShouldEqual("iso-8859-5");
        }

        [Fact]
        public void Should_parse_accept_charset_header_values_when_containing_multiple_values()
        {
            // Given
            var values = new[] { "utf-8, iso-8859-5" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Charset", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptCharset.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].Item1.ShouldEqual("utf-8");
            headers[1].Item1.ShouldEqual("iso-8859-5");
        }

        [Fact]
        public void Should_parse_accept_charset_header_value_quality_when_available()
        {
            // Given
            var values = new[] { "utf-8;q=0.3" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Charset", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptCharset.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("utf-8");
            headers[0].Item2.ShouldEqual(0.3m);
        }

        [Fact]
        public void Should_default_accept_charset_header_values_to_quality_one_if_not_explicitly_defined()
        {
            // Given
            var values = new[] { "utf-8" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Charset", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptCharset.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("utf-8");
            headers[0].Item2.ShouldEqual(1m);
        }

        [Fact]
        public void Should_ignore_accept_charset_header_values_with_invalid_quality()
        {
            // Given
            var values = new[] { "utf-8, iso-8859-5;q=a" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Charset", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptCharset.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("utf-8");
        }

        [Fact]
        public void Should_sort_accept_charset_header_values_decending_based_on_quality()
        {
            // Given
            var values = new[] { "utf-8;q=0.3, iso-8859-5, iso-8859-15;q=0.7" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Charset", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptCharset.ToList();

            // Then
            headers.Count.ShouldEqual(3);
            headers[0].Item1.ShouldEqual("iso-8859-5");
            headers[1].Item1.ShouldEqual("iso-8859-15");
            headers[2].Item1.ShouldEqual("utf-8");
        }

        [Theory]
        [InlineData("accept-charset")]
        [InlineData("AcCepT-cHaRsET")]
        public void Should_ignore_case_of_accept_charset_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "utf-8", "iso-8859-5" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptCharset.ToList();

            // Then
            headers[0].Item1.ShouldEqual("utf-8");
            headers[1].Item1.ShouldEqual("iso-8859-5");
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_encoding_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.AcceptEncoding.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_accept_encoding_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "compress", "sdch" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Encoding", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptEncoding.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].ShouldEqual("compress");
            headers[1].ShouldEqual("sdch");
        }

        [Fact]
        public void Should_parse_accept_encoding_header_values_when_containing_multiple_values()
        {
            // Given
            var values = new[] { "compress, sdch" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Encoding", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptEncoding.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].ShouldEqual("compress");
            headers[1].ShouldEqual("sdch");
        }

        [Theory]
        [InlineData("accept-encoding")]
        [InlineData("AcCepT-ENcOdinG")]
        public void Should_ignore_case_of_accept_encoding_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "compress", "sdch" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptEncoding.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].ShouldEqual("compress");
            headers[1].ShouldEqual("sdch");
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_language_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_accept_language_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "en-US", "sv-SE" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Language", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].Item1.ShouldEqual("en-US");
            headers[1].Item1.ShouldEqual("sv-SE");
        }

        [Fact]
        public void Should_parse_accept_language_header_values_when_containing_multiple_values()
        {
            // Given
            var values = new[] { "en-US, sv-SE" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Language", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].Item1.ShouldEqual("en-US");
            headers[1].Item1.ShouldEqual("sv-SE");
        }

        [Fact]
        public void Should_parse_accept_language_header_value_quality_when_available()
        {
            // Given
            var values = new[] { "en-US;q=0.3" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Language", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("en-US");
            headers[0].Item2.ShouldEqual(0.3m);
        }

        [Fact]
        public void Should_default_accept_language_header_values_to_quality_one_if_not_explicitly_defined()
        {
            // Given
            var values = new[] { "en-US" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Language", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("en-US");
            headers[0].Item2.ShouldEqual(1m);
        }

        [Fact]
        public void Should_ignore_accept_language_header_values_with_invalid_quality()
        {
            // Given
            var values = new[] { "en-US, sv-SE;q=a" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Language", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(1);
            headers[0].Item1.ShouldEqual("en-US");
        }

        [Fact]
        public void Should_sort_accept_language_header_values_decending_based_on_quality()
        {
            // Given
            var values = new[] { "en-US;q=0.3, da, sv-SE;q=0.7" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Accept-Language", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(3);
            headers[0].Item1.ShouldEqual("da");
            headers[1].Item1.ShouldEqual("sv-SE");
            headers[2].Item1.ShouldEqual("en-US");
        }

        [Theory]
        [InlineData("accept-language")]
        [InlineData("AcCepT-LaNGUage")]
        public void Should_ignore_case_of_accept_language_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "en-US", "sv-SE" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders).AcceptLanguage.ToList();

            // Then
            headers.Count.ShouldEqual(2);
        }
        
        [Fact]
        public void Should_return_empty_string_when_authorization_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Authorization.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_authorization_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Authorization", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Authorization.ShouldBeSameAs(expectedValues[0]);
        }

        [Theory]
        [InlineData("authorization")]
        [InlineData("AutHORizaTion")]
        public void Should_ignore_case_of_authorization_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Authorization.ShouldBeSameAs(expectedValues[0]);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_cache_control_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.CacheControl.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_cache_control_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "public", "max-age=123445" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Cache-Control", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.CacheControl.ShouldBeSameAs(expectedValues);
        }

        [Theory]
        [InlineData("cache-control")]
        [InlineData("CaCHe-ContROL")]
        public void Should_ignore_case_of_cache_control_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "public", "max-age=123445" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.CacheControl.ShouldBeSameAs(expectedValues);
        }

        [Fact]
        public void Should_return_empty_string_when_connection_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Connection.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_connection_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "closed" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Connection", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Connection.ShouldBeSameAs(expectedValues[0]);
        }

        [Theory]
        [InlineData("connection")]
        [InlineData("CONNecTION")]
        public void Should_ignore_case_of_connection_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "closed" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Connection.ShouldBeSameAs(expectedValues[0]);
        }

        [Fact]
        public void Should_return_zero_when_content_length_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.ContentLength.ShouldEqual(0L);
        }

        [Fact]
        public void Should_return_content_length_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "12345" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Content-Length", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.ContentLength.ShouldEqual(12345L);
        }

        [Theory]
        [InlineData("content-length")]
        [InlineData("CoNTEnt-LENGth")]
        public void Should_ignore_case_of_content_length_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "12345" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.ContentLength.ShouldEqual(12345L);
        }

        [Fact]
        public void Should_return_empty_string_when_content_type_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.ContentType.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_content_type_header_when_available()
        {
            // Given
            var expectedValues = new[] { "text/ninja" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Content-Type", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.ContentType.ShouldEqual("text/ninja");
        }

        [Theory]
        [InlineData("content-type")]
        [InlineData("CoNTEnt-tYPe")]
        public void Should_ignore_case_of_content_type_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "text/ninja" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.ContentType.ShouldEqual("text/ninja");
        }

        [Fact]
        public void Should_return_null_date_when_date_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Date.ShouldBeNull();;
        }


        [Fact]
        public void Should_return_date_when_date_headers_available()
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var expectedValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Date", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Date.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_be_null_when_date_headers_are_invalid()
        {
            // Given
            var expectedValues = new[] { "Bad Date Header" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Date", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Date.ShouldBeNull();
        }

        [Fact]
        public void Should_return_empty_enumerable_when_cookie_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Cookie.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_cookie_headers_when_available()
        {
            // Given
            var rawValues = new[] { "foo=bar", "name=value" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Cookie", rawValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            ValidateCookie(headers.Cookie.First(), "foo", "bar");
            ValidateCookie(headers.Cookie.Last(), "name", "value");
        }

        [Theory]
        [InlineData("cookie")]
        [InlineData("COokIE")]
        public void Should_ignore_case_of_cookie_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "foo=bar", "name=value" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            ValidateCookie(headers.Cookie.First(), "foo", "bar");
            ValidateCookie(headers.Cookie.Last(), "name", "value");
        }

        [Theory]
        [InlineData("date")]
        [InlineData("daTE")]
        public void Should_ignore_case_of_date_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var expectedValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Date.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_empty_string_when_host_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Host.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_host_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "en.wikipedia.org" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Host", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Host.ShouldBeSameAs(expectedValues[0]);
        }

        [Theory]
        [InlineData("host")]
        [InlineData("hOsT")]
        public void Should_ignore_case_of_host_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "en.wikipedia.org" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Host.ShouldBeSameAs(expectedValues[0]);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_ifmatch_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfMatch.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_ifmatch_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "xyzzy", "c3piozzzz" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-Match", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfMatch.ShouldBeSameAs(expectedValues);
        }

        [Fact]
        public void Should_parse_ifmatch_header_values_when_containing_multiple_values()
        {
            // Given
            var values = new[] { "xyzzy", "c3piozzzz" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-Match", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).IfMatch.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].ShouldEqual("xyzzy");
            headers[1].ShouldEqual("c3piozzzz");
        }

        [Theory]
        [InlineData("if-match")]
        [InlineData("If-MaTCH")]
        public void Should_ignore_case_of_ifmatch_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "xyzzy", "c3piozzzz" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfMatch.ShouldBeSameAs(expectedValues);
        }

        [Fact]
        public void Should_return_null_when_ifmodifiedsince_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfModifiedSince.ShouldBeNull();
        }

        [Fact]
        public void Should_return_ifmodifiedsince_when_available()
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var expectedValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-Modified-Since", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfModifiedSince.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_null_when_ifmodifiedsince_when_invalid()
        {
            // Given
            var expectedValues = new[] { "Bad Date" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-Modified-Since", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfModifiedSince.ShouldBeNull();
        }

        [Theory]
        [InlineData("if-modified-since")]
        [InlineData("IF-MODIFIED-SINCE")]
        public void Should_ignore_case_of_ifmodifiedsince_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var expectedValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfModifiedSince.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_ifnonematch_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfNoneMatch.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_ifnonematch_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "xyzzy", "c3piozzzz" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-None-Match", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfNoneMatch.ShouldBeSameAs(expectedValues);
        }

        [Fact]
        public void Should_parse_ifnonematch_header_values_when_containing_multiple_values()
        {
            // Given
            var values = new[] { "xyzzy", "c3piozzzz" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-None-Match", values } };

            // When
            var headers = new RequestHeaders(rawHeaders).IfNoneMatch.ToList();

            // Then
            headers.Count.ShouldEqual(2);
            headers[0].ShouldEqual("xyzzy");
            headers[1].ShouldEqual("c3piozzzz");
        }

        [Theory]
        [InlineData("if-none-match")]
        [InlineData("If-NONe-MaTCH")]
        public void Should_ignore_case_of_ifnonematch_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "xyzzy", "c3piozzzz" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfNoneMatch.ShouldBeSameAs(expectedValues);
        }

        [Fact]
        public void Should_return_empty_string_when_ifrange_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfRange.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_ifrange_header_when_available()
        {
            // Given
            var expectedValues = new[] { "737060cd8c284d8af7ad3082f209582d" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-Range", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfRange.ShouldEqual(expectedValues[0]);
        }

        [Theory]
        [InlineData("if-range")]
        [InlineData("IF-RANGe")]
        public void Should_ignore_case_of_ifrange_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "737060cd8c284d8af7ad3082f209582d" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfRange.ShouldEqual(expectedValues[0]);
        }

        [Fact]
        public void Should_return_null_when_ifunmodifiedsince_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfUnmodifiedSince.ShouldBeNull();
        }

        [Fact]
        public void Should_return_date_ifunmodifiedsince_when_available()
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var expectedValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-Unmodified-Since", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfUnmodifiedSince.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_null_ifunmodifiedsince_is_invalid()
        {
            // Given
            var expectedValues = new[] { "Bad Date" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "If-Unmodified-Since", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfUnmodifiedSince.ShouldBeNull();
        }

        [Theory]
        [InlineData("if-unmodified-since")]
        [InlineData("If-UnmoDified-SInce")]
        public void Should_ignore_case_of_ifunmodifiedsince_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var expectedValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.IfUnmodifiedSince.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_zero_when_maxforwards_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.MaxForwards.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_maxforwards_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "12" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Max-Forwards", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.MaxForwards.ShouldEqual(12);
        }

        [Theory]
        [InlineData("max-forwards")]
        [InlineData("MAX-Forwards")]
        public void Should_ignore_case_of_maxforwards_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "12" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.MaxForwards.ShouldEqual(12);
        }

        [Fact]
        public void Should_return_empty_string_when_referer_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Referrer.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_referer_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "http://nancyfx.org" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "Referer", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Referrer.ShouldBeSameAs(expectedValues[0]);
        }

        [Theory]
        [InlineData("referer")]
        [InlineData("RefERer")]
        public void Should_ignore_case_of_referer_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "http://nancyfx.org" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.Referrer.ShouldBeSameAs(expectedValues[0]);
        }

        [Fact]
        public void Should_return_empty_string_when_useragent_headers_are_not_available()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.UserAgent.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_useragent_headers_when_available()
        {
            // Given
            var expectedValues = new[] { "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.815.0 Safari/535.1" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "User-Agent", expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.UserAgent.ShouldBeSameAs(expectedValues[0]);
        }

        [Theory]
        [InlineData("user-agent")]
        [InlineData("user-AGENT")]
        public void Should_ignore_case_of_useragent_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedValues = new[] { "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.815.0 Safari/535.1" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { headerName, expectedValues } };

            // When
            var headers = new RequestHeaders(rawHeaders);

            // Then
            headers.UserAgent.ShouldBeSameAs(expectedValues[0]);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_header_not_available_using_indexer()
        {
            // Given
            var rawHeaders = new Dictionary<string, IEnumerable<string>>();
            var headers = new RequestHeaders(rawHeaders);

            // When
            var result = headers["not-found"];

            // Then
            result.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_header_values_when_available_using_indexer()
        {
            // Given
            var expectedValues = new[] { "fakeValue1", "fakeValue2" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "some-header", expectedValues } };
            var headers = new RequestHeaders(rawHeaders);

            // When
            var result = headers["some-header"];

            // Then
            result.ShouldBeSameAs(expectedValues);
        }

        [Fact]
        public void Should_ignore_case_when_available_using_indexer()
        {
            // Given
            var expectedValues = new[] { "fakeValue1", "fakeValue2" };
            var rawHeaders = new Dictionary<string, IEnumerable<string>> { { "some-header", expectedValues } };
            var headers = new RequestHeaders(rawHeaders);

            // When
            var result = headers["sOme-HeAdEr"];

            // Then
            result.ShouldBeSameAs(expectedValues);
        }

        private static void ValidateCookie(INancyCookie cookie, string name, string value)
        {
            cookie.Name.ShouldEqual(name);
            cookie.Value.ShouldEqual(value);
        }
    }
}