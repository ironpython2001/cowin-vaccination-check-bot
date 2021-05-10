using RestSharp;
using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TextCopy;
using System.Text;

namespace cowinvaccinecheck
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var inputData = new List<VaccineCheckInput>
            {
                new VaccineCheckInput(){Date ="11-05-2021", District="581",DistrictName="Hyderabad" },
                new VaccineCheckInput(){Date ="12-05-2021", District="581",DistrictName="Hyderabad" },
                new VaccineCheckInput(){Date ="11-05-2021", District="596",DistrictName="Medchal" },
                new VaccineCheckInput(){Date ="12-05-2021", District="596",DistrictName="Medchal" },
                new VaccineCheckInput(){Date ="11-05-2021", District="604",DistrictName="Sangareddy" },
                new VaccineCheckInput(){Date ="12-05-2021", District="604",DistrictName="Sangareddy" }
            };
            var result = check(inputData);
            
            if (result.Count == 0)
            {
                Console.WriteLine("Not available");
            }
            else
            {
                var clipboardText =new StringBuilder();
                foreach (var r in result)
                {
                    Console.WriteLine(r);
                    clipboardText.Append(r + Environment.NewLine);
                    
                }
                await ClipboardService.SetTextAsync(clipboardText.ToString());
            }
        }

        private static List<string> check(List<VaccineCheckInput> input)
        {
            var result = new List<string>();
            foreach (var i in input)
            {
                Console.WriteLine($"checking for the date {i.Date} and district {i.DistrictName}");
                var vaccineDate = i.Date;
                var vaccineDistrict = i.District;
                var client = new RestClient($"https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id={vaccineDistrict}&date={vaccineDate}");

                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AlwaysMultipartFormData = true;
                var response = client.Execute(request);
                var data = JsonConvert.DeserializeObject<Rootobject>(response.Content);
                

                foreach (var c in data.centers)
                {
                    foreach (var s in c.sessions)
                    {
                        if (s.available_capacity > 0)
                        {
                            result.Add($"Available Date:{s.date}");
                            result.Add($"PinCode:{c.pincode},DistrictName:{i.DistrictName}");
                            result.Add($"Vaccine:{s.vaccine}");
                            result.Add($"HospitalName:{c.name}");
                            result.Add($"No Of Available:{s.available_capacity}");
                            result.Add($"Fees:{c.fee_type}");
                            result.Add("****************");
                        }
                    }
                }
            }
            return result;
        }
    }

}
