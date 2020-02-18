using System;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Web.Autofac;

namespace Test.Integration {
   [TestClass]
   public class TestUrlEncodeTransform {
      [TestMethod]
      public void TestUrlEncode() {
         const string xml = @"<add name='Test'>
  <connections>
    <add name='input' provider='bogus' seed='1' />
  </connections>
  <entities>
    <add name='Contact' size='1'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
      <calculated-fields>
         <add name='Url' length='2000' t='format(http://google.com/search?q=transformalize&Identity={Identity}&FirstName={FirstName})' />
         <add name='Encoded' length='2000' t='copy(Url).urlEncode()' />
      </calculated-fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer(new WebTransformModule()).CreateScope(xml, logger)) {

            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new WebTransformModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               Assert.AreEqual(1, process.Entities.First().Rows.Count);
               Assert.AreEqual("http://google.com/search?q=transformalize&Identity=1&FirstName=Justin", process.Entities.First().Rows[0]["Url"]);
               Assert.AreEqual("http%3A%2F%2Fgoogle.com%2Fsearch%3Fq%3Dtransformalize%26Identity%3D1%26FirstName%3DJustin", process.Entities.First().Rows[0]["Encoded"]);

            }
         }
      }
   }
}
