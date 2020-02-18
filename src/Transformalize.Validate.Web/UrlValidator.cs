using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Transformalize.Validators;

namespace Transformalize.Validate.Web {

   public class UrlValidator : StringValidate {
      private readonly BetterFormat _betterFormat;
      private readonly Field _input;

      public UrlValidator(IContext context = null) : base(context) {
         if (IsMissingContext()) {
            return;
         }

         if (!Run)
            return;

         _input = SingleInput();

         var help = Context.Field.Help;
         if (help == string.Empty) {
            help = $"{Context.Field.Label} must be an url.";
         }
         _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
      }
      public override IRow Operate(IRow row) {
         // https://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
         var value = GetString(row, _input);
         var valid = Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

         if (IsInvalid(row, valid)) {
            AppendMessage(row, _betterFormat.Format(row));
         }

         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("url");
      }

   }
}
