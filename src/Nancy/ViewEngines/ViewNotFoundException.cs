namespace Nancy.ViewEngines
{
    using System;
    using System.Linq;

    public class ViewNotFoundException : Exception
    {
        public string ViewName { get; private set; }

        public string[] AvailableViewEngineExtensions { get; private set; }

    	private string message = string.Empty;

        public ViewNotFoundException(string viewName, string[] availableViewEngineExtensions)
        {
            this.ViewName = viewName;
            this.AvailableViewEngineExtensions = availableViewEngineExtensions;

			message = String.Format(
					"Unable to locate view '{0}'. Currently available view engine extensions: {1}",
					this.ViewName,
					this.AvailableViewEngineExtensions.Aggregate((s1, s2) => s1 + "," + s2));
        }

		public ViewNotFoundException(string viewName)
		{
			message = string.Format("Unable to locate view {0}", viewName);
		}

        public override string Message
        {
            get { return message; }
        }
    }
}