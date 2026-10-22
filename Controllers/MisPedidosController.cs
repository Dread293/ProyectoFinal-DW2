using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


//inge falta diseñar los controladores

namespace ProyectoFinal
{
    [Route("[controller]")]
    public class MisPedidosController : Controller
    {
        private readonly ILogger<MisPedidosController> _logger;

        public MisPedidosController(ILogger<MisPedidosController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}

//Ing. Falta hacer la implementacion del diseño 