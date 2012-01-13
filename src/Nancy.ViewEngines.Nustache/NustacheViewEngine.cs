﻿using System.Dynamic;

namespace Nancy.ViewEngines.Nustache
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using global::Nustache.Core;
    using Nancy.Responses;

    /// <summary>
    /// View engine for rendering nustache views.
    /// </summary>
    public class NustacheViewEngine : IViewEngine
    {
        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return new[] { "nustache" }; }
        }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        private Template GetOrCompileTemplate(ViewLocationResult viewLocationResult, IRenderContext renderContext)
        {
            var viewFactory = renderContext.ViewCache.GetOrAdd(
                viewLocationResult,
                x => this.GetCompiledTemplate<dynamic>(x.Contents.Invoke()));

            var view = viewFactory.Invoke();

            return view;
        }

        private Func<Template> GetCompiledTemplate<TModel>(TextReader reader)
        {
            var template = new Template();
            template.Load(reader);

            return () =>{
                return template;
            };
        }

    	/// <summary>
    	/// Renders the view.
    	/// </summary>
    	/// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
    	/// <param name="model">The model that should be passed into the view</param>
    	/// <param name="renderContext"></param>
    	/// <param name="viewBag">Extra data available to a view</param>
    	/// <returns>A response</returns>
		public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext, ExpandoObject viewBag = null)
        {
            return new HtmlResponse
            {
                Contents = stream =>
                {
                    var template =
                        this.GetOrCompileTemplate(viewLocationResult, renderContext);

                    var writer =
                        new StreamWriter(stream);

                    template.Render(model, writer, null);
                }
            };
        }
    }
}
