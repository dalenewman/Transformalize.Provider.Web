using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Transformalize.Validators;

namespace Transformalize.Validate.Web {

   public class EmailValidator : StringValidate {
      private readonly BetterFormat _betterFormat;
      private readonly Field _input;

      public EmailValidator(IContext context = null) : base(context) {
         if (IsMissingContext()) {
            return;
         }

         if (!Run)
            return;

         _input = SingleInput();

         var help = Context.Field.Help;
         if (help == string.Empty) {
            help = $"{Context.Field.Label} must be an email.";
         }
         _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
      }
      public override IRow Operate(IRow row) {
         bool valid = false;
         var value = GetString(row, _input);
         try {
            var addr = new System.Net.Mail.MailAddress(value);
            valid = addr.Address == value;
         } catch (FormatException ex) {
         } finally {
            if (IsInvalid(row, valid)) {
               AppendMessage(row, _betterFormat.Format(row));
            }
         }


         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("email");
      }

   }
}
