using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web;

namespace TestWcfService
{
    public class ApiKeyValidationBehaviorExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new ApiKeyValidationBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(ApiKeyValidationBehavior); }
        }
    }
}