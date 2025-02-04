using System.Net;
using System.Net.Http;
using System.Web.Http;
using SwtyChina;

namespace WebApplication2.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get(string date = "new")
        {
            //Swty s = new Swty();
            if (date == "new")
            {
                return Request.CreateResponse(HttpStatusCode.OK, Swty.GetNewItems());
            }
            else
            {
                string[] arr = date.Split('-'); // e.g.: date=yyyyy-mm-reverse; date=[search string]+content+xmlonly
                if (arr.Length < 2)
                    return Request.CreateResponse(HttpStatusCode.OK, Swty.Search(date));
                bool bReverse = arr.Length < 3 ? false : arr[arr.Length - 1].ToLower() == "reverse";
                return Request.CreateResponse(HttpStatusCode.OK, Swty.GetMonthItems(arr[0], arr[1], bReverse));
            }
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
