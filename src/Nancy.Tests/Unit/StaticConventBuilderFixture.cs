using System;
using System.IO;
using System.Text;
using FakeItEasy;
using Nancy.Conventions;
using Nancy.Responses;
using Xunit;

namespace Nancy.Tests.Unit
{
	public class StaticConventBuilderFixture
	{
		private const string StylesheetContents = @"body {
	background-color: white;
}";

		[Fact]
		public void Static_routes_can_have_same_name_as_extension()
		{
			var nancyCtx = new NancyContext(){ Request = new Request("GET", "css/styles.css", "http")};

			var resolver = StaticContentConventionBuilder.AddDirectory("css", @"Resources\Assets\Styles");

			GenericFileResponse.SafePaths.Add(Environment.CurrentDirectory);
			var response = resolver.Invoke(nancyCtx, Environment.CurrentDirectory) as GenericFileResponse;

			Assert.NotNull(response);
			Assert.True("styles.css".Equals(response.Filename, StringComparison.CurrentCultureIgnoreCase));

			using (var ms = new MemoryStream())
			{
				response.Contents(ms);
				var css = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
				Assert.Equal(StylesheetContents, css);
			}
		}
	}
}