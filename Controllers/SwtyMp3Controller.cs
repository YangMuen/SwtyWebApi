using System.Net;
using System.Net.Http;
using System.Web.Http;
using SwtyChina;

namespace WebApplication2.Controllers
{
    public class SwtyMp3Controller : ApiController
    {
        // GET api/swtymp3
        public HttpResponseMessage Get(string path = "")
        {
            return Request.CreateResponse(HttpStatusCode.OK, SwtyMp3.GetDirContents(path));
        }

        // GET api/swtymp3/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/swtymp3
        public void Post([FromBody]string value)
        {
        }

        // PUT api/swtymp3/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/swtymp3/5
        public void Delete(int id)
        {
        }
    }
}