using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web;

namespace TestWcfService
{
    public class JwtTokenValidationBehaviorExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new JwtTokenValidationBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(JwtTokenValidationBehavior); }
        }
    }
}