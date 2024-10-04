namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using Xunit.Sdk;

    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "No need to access culture name.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UsingCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly string cultureName;

        private CultureInfo? originalCulture;
        private CultureInfo? originalUiCulture;

        public UsingCultureAttribute(string cultureName)
        {
            this.cultureName = cultureName;
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (this.originalCulture is CultureInfo originalCulture)
            {
                CultureInfo.CurrentCulture = originalCulture;
                this.originalCulture = null;
            }

            if (this.originalUiCulture is CultureInfo originalUiCulture)
            {
                CultureInfo.CurrentUICulture = originalUiCulture;
                this.originalUiCulture = null;
            }
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            this.originalCulture = CultureInfo.CurrentCulture;
            this.originalUiCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentCulture = new CultureInfo(this.cultureName);
            CultureInfo.CurrentUICulture = new CultureInfo(this.cultureName);
        }
    }
}
