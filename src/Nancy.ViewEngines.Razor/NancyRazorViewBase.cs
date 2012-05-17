﻿namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Nancy.Helpers;

    /// <summary>
    /// Base class for nancy razor views.
    /// </summary>
    public abstract class NancyRazorViewBase
    {
        private readonly StringBuilder contents;
        private string childBody;
        private IDictionary<string, string> childSections;

        /// <summary>
        /// Gets the body.
        /// </summary>
        public String Body { get; private set; }

        /// <summary>
        /// Gets or sets the section contents.
        /// </summary>
        /// <value>
        /// The section contents.
        /// </value>
        public IDictionary<string, string> SectionContents { get; set; }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>
        /// The layout.
        /// </value>
        public string Layout { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has layout.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has layout; otherwise, <c>false</c>.
        /// </value>
        public bool HasLayout
        {
            get
            {
                return !String.IsNullOrEmpty(this.Layout);
            }
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public IDictionary<string, Action> Sections { get; set; }

        /// <summary>
        /// Executes the view.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Initializes the specified engine.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="model">The model.</param>
        public virtual void Initialize(RazorViewEngine engine, IRenderContext renderContext, object model)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyRazorViewBase"/> class.
        /// </summary>
        protected NancyRazorViewBase()
        {
            this.Sections = new Dictionary<string, Action>();
            this.contents = new StringBuilder();
        }

        /// <summary>
        /// Writes the results of expressions like: "@foo.Bar"
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void Write(object value)
        {
            WriteLiteral(HtmlEncode(value));
        }

        /// <summary>
        /// Writes literals like markup: "<p>Foo</p>"
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void WriteLiteral(object value)
        {
            contents.Append(value);
        }

        /// <summary>
        /// Stores sections
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="action">The action.</param>
        public virtual void DefineSection(string sectionName, Action action)
        {
            this.Sections.Add(sectionName, action);
        }

        /// <summary>
        /// Renders the section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public virtual object RenderSection(string sectionName)
        {
            return this.RenderSection(sectionName, true);
        }

        /// <summary>
        /// Renders the section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        public virtual object RenderSection(string sectionName, bool required)
        {
            string sectionContent;

            var exists = this.childSections.TryGetValue(sectionName, out sectionContent);
            if (!exists && required)
            {
                throw new InvalidOperationException("Section name " + sectionName + " not found and is required.");
            }

            this.contents.Append(sectionContent ?? String.Empty);

            return null;
        }

        /// <summary>
        /// Renders the body.
        /// </summary>
        /// <returns></returns>
        public virtual object RenderBody()
        {
            this.contents.Append(this.childBody);

            return null;
        }

        ///<summary>
        ///Indicates if a section is defined.
        ///</summary>
        public virtual bool IsSectionDefined(string sectionName)
        {
            return this.childSections.ContainsKey(sectionName);
        }

        /// <summary>
        /// Executes the view.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="sectionContents">The section contents.</param>
        public void ExecuteView(string body, IDictionary<string, string> sectionContents)
        {
            this.childBody = body ?? string.Empty;
            this.childSections = sectionContents ?? new Dictionary<string, string>();
            try
            {
                this.Execute();
            }
            catch (NullReferenceException)
            {
                throw new ViewRenderException("Unable to render the view.  Most likely the Model, or a property on the Model is null");
            }

            this.Body = this.contents.ToString();

            this.SectionContents = new Dictionary<string, string>(this.Sections.Count);
            foreach (var section in this.Sections)
            {
                this.contents.Clear();
                try
                {
                    section.Value.Invoke();
                }
                catch (NullReferenceException e)
                {
                    throw new ViewRenderException(string.Format("Unable to render the section {0}.  Does the section require a model (it may be null)?" + section.Key));
                }
                
                this.SectionContents.Add(section.Key, this.contents.ToString());
            }
        }

        /// <summary>
        /// Html encodes an object if required
        /// </summary>
        /// <param name="value">Object to potentially encode</param>
        /// <returns>String representation, encoded if necessary</returns>
        private static string HtmlEncode(object value)
        {
            if (value == null)
            {
                return null;
            }

            var str = value as IHtmlString;

            return str != null ? str.ToHtmlString() : HttpUtility.HtmlEncode(Convert.ToString(value, CultureInfo.CurrentCulture));
        }
    }

    /// <summary>
    /// A strongly-typed view base.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class NancyRazorViewBase<TModel> : NancyRazorViewBase
    {
        /// <summary>
        /// Gets the Html helper.
        /// </summary>
        public IHtmlHelpers<TModel> Html { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TModel Model { get; private set; }

        /// <summary>
        /// Gets the Url helper.
        /// </summary>
        public IUrlHelpers<TModel> Url { get; private set; }

        /// <summary>
        /// Initializes the specified engine.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="model">The model.</param>
        public override void Initialize(RazorViewEngine engine, IRenderContext renderContext, object model)
        {
            Html = new HtmlHelpers<TModel>(engine, renderContext, (TModel)model);
            Model = (TModel)model;
            Url = new UrlHelpers<TModel>(engine, renderContext);
        }
    }
}
