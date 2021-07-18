using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace faasSzero
{
    public class FunctionWatcher
    {
        private static string LabelScaleZero = "com.openfaas.scale.zero";
        private OpenFaaSApi.Api Api;
        private Dictionary<string, OpenFaaSApi.Model.Function> lastCheck;

        public async Task Check()
        {

            List<OpenFaaSApi.Model.Function> functions = null;
            try {
                functions = await Api.GetFunctionsAsync();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            if (functions == null)
                return;

            Dictionary<string, OpenFaaSApi.Model.Function> functionsDic = new Dictionary<string, OpenFaaSApi.Model.Function>();
            foreach (OpenFaaSApi.Model.Function fn in functions) {
                functionsDic[fn.Name] = fn;

                if (lastCheck != null) {
                    OpenFaaSApi.Model.Function lastFnStatus = lastCheck.GetValueOrDefault(fn.Name, null);
                    if (lastFnStatus != null) {

                        string isScaleToZero = fn.Labels.GetValueOrDefault(LabelScaleZero, "false").ToLower();
                        if (lastFnStatus.InvocationCount == fn.InvocationCount
                            && fn.Replicas > 0
                            && isScaleToZero == "true") {
                            try {
                                Console.Write("Scale function {0} to zero", fn.Name);
                                await Api.ScaleAsync(fn.Name, 0);
                                Console.WriteLine("\t- Success");
                            } catch(Exception e) {
                                Console.WriteLine("\t- Failed {0}", e.Message);
                            }
                        }
                    }
                }
            }

            lastCheck = functionsDic;
        }

        public FunctionWatcher(string gateway, string user, string password)
        {
            Api = new OpenFaaSApi.Api(gateway, user, password);
        }
    }
}
