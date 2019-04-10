#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Autofac;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;

namespace Transformalize.Providers.Web.Autofac {
   public class WebProviderModule : Module {

      private readonly string Provider = "web";
      protected override void Load(ContainerBuilder builder) {

         if (!builder.Properties.ContainsKey("Process")) {
            return;
         }

         var process = (Process)builder.Properties["Process"];

         foreach (var action in process.Actions.Where(a => a.Type == Provider)) {
            builder.Register<IAction>((c, p) => new WebAction(c.Resolve<IContext>(), action)).Named<IAction>(action.Key);
         }

         // Connections
         foreach (var connection in process.Connections.Where(c => c.Provider == Provider)) {
            // Schema Reader
            builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
         }

         // entity input
         foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Connection).Provider == Provider)) {

            // input version detector
            builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // input read
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

               switch (input.Connection.Provider) {
                  case "web":
                     if (input.Connection.Delimiter == string.Empty && input.Entity.Fields.Count(f => f.Input) == 1) {
                        return new WebReader(input, rowFactory);
                     }
                     return new WebCsvReader(input, rowFactory);
                  default:
                     return new NullReader(input, false);
               }
            }).Named<IRead>(entity.Key);

         }

         // TODO: be able to post (write) to web
         if (process.Output().Provider == "web") {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in process.Entities) {

               // ENTITY OUTPUT CONTROLLER
               builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

               // ENTITY WRITER
               builder.Register<IWrite>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  return new NullWriter(output);
               }).Named<IWrite>(entity.Key);
            }
         }
      }
   }
}