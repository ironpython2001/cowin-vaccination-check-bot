using RestSharp;
using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace cowinvaccinecheck
{
    class Program
    {
        static void Main(string[] args)
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
                foreach (var r in result)
                {
                    Console.WriteLine(r);
                }
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



    public class VaccineCheckInput
    {
        public string Date { get; set; }
        public string District { get; set; }
        public string DistrictName { get; set; }
    }

    public class Rootobject
    {
        public Center[] centers { get; set; }
    }

    public class Center
    {
        public int center_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string state_name { get; set; }
        public string district_name { get; set; }
        public string block_name { get; set; }
        public int pincode { get; set; }
        public int lat { get; set; }
        public int _long { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string fee_type { get; set; }
        public Session[] sessions { get; set; }
        public Vaccine_Fees[] vaccine_fees { get; set; }
    }

    public class Session
    {
        public string session_id { get; set; }
        public string date { get; set; }
        public int available_capacity { get; set; }
        public int min_age_limit { get; set; }
        public string vaccine { get; set; }
        public string[] slots { get; set; }
    }

    public class Vaccine_Fees
    {
        public string vaccine { get; set; }
        public string fee { get; set; }
    }

}
