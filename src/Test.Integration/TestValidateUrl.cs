using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Validate.Web.Autofac;

namespace Test.Integration {
   [TestClass]
   public class TestValidateUrl {
      [TestMethod]
      public void TestValidate() {
         const string xml = @"<add name='Test'>
  <entities>
    <add name='Test'>
      <rows>
         <add url='https://www.google.com' />
         <add url='javascript:window.location=https://www.google.com'/>
      </rows>
      <fields>
        <add name='url' type='string' v='url()' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer(new WebValidateModule()).CreateScope(xml, logger)) {

            var process = outer.Resolve<Process>();
            using (var inner = new Container(new WebValidateModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               var rows = process.Entities.First().Rows;
               Assert.AreEqual(2, rows.Count);
               Assert.AreEqual(true, rows[0]["urlValid"]);
               Assert.AreEqual(false, rows[1]["urlValid"]);
               Assert.AreEqual("url must be an url.|", rows[1]["urlMessage"]);

            }
         }
      }
   }
}
