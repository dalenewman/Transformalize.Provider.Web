using System;
using System.Collections.Generic;
using Autofac;
using Cfg.Net.Shorthand;
using Transformalize.Contracts;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Validate.Web.Autofac {
   public class WebValidateModule : Module {

      private HashSet<string> _methods;
      private ShorthandRoot _shortHand;

      protected override void Load(ContainerBuilder builder) {

         // get methods and shorthand from builder
         _methods = builder.Properties.ContainsKey("Methods") ? (HashSet<string>)builder.Properties["Methods"] : new HashSet<string>();
         _shortHand = builder.Properties.ContainsKey("ShortHand") ? (ShorthandRoot)builder.Properties["ShortHand"] : new ShorthandRoot();

         RegisterValidator(builder, (ctx, c) => new EmailValidator(c), new EmailValidator().GetSignatures());
      }

      private void RegisterValidator(ContainerBuilder builder, Func<IComponentContext, IContext, IValidate> getValidator, IEnumerable<OperationSignature> signatures) {

         foreach (var s in signatures) {
            if (_methods.Add(s.Method)) {

               var method = new Method { Name = s.Method, Signature = s.Method, Ignore = s.Ignore };
               _shortHand.Methods.Add(method);

               var signature = new Signature {
                  Name = s.Method,
                  NamedParameterIndicator = s.NamedParameterIndicator
               };

               foreach (var parameter in s.Parameters) {
                  signature.Parameters.Add(new Parameter {
                     Name = parameter.Name,
                     Value = parameter.Value
                  });
               }
               _shortHand.Signatures.Add(signature);
            }

            builder.Register((ctx, p) => getValidator(ctx, p.Positional<IContext>(0))).Named<IValidate>(s.Method);
         }
      }
   }
}
