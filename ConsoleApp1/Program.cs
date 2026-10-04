using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Channels;
using System.Timers;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace REPO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? volver;
            string? respuesta;
            const string archivo = "monstruos.json";
            string ruta = Path.Combine(AppContext.BaseDirectory, archivo);
            List<Monstruo> monstruos = Importar<List<Monstruo>>(ruta);
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.WriteLine("\n                           MONSTRUOS DEL R.E.P.O");
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            do {
                respuesta = ComprobarValor<string>("\n\nQue quieres hacer? Escribe el número: \n\n1.Guardar un monstruo nuevo | 2.Consultar Monstruo | 3.Borrar Monstruo | 4.Buscar por característica: ", Nulovacio, "Por favor escribe algo: ");
                if (respuesta != "1" && respuesta != "2" && respuesta != "3" && respuesta != "4") Console.WriteLine("\nNo has introducido un valor apto...");
                else
                {
                    Action Eleccion = respuesta switch
                    {
                        "1" => () => CrearYAñadirMonstruo(ref monstruos),
                        "2" => () => ConsultarMonstruo(monstruos),
                        "3" => () => Eliminar(ref monstruos),
                        "4" => () => BuscarPorCaracteristica(monstruos)
                    };
                    Eleccion();
                    do
                    {
                        Console.Write("\n¿Te gustaria realizar otra operación? SI/NO: ");
                        volver = Console.ReadLine()!.ToUpper()!;
                    } while (volver != "SI" && volver != "NO");
                    if (volver == "NO") break;
                }
            } while (true);
            Guardar(monstruos, ruta);
            Console.WriteLine("\n\n              *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.WriteLine("\n               GRACIAS POR USAR LA BASE DE DATOS DE R.E.P.O");
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.ReadKey();
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void CrearYAñadirMonstruo(ref List<Monstruo> monstruos)
        {
            string? nombre;
            int tier;
            int vida;
            int daño;
            Console.Write("\nESCRIBE 'X' EN CUALQUIER MOMENTO PARA CANCELAR");
            nombre = ComprobarValor<string>("\nDime el nombre: ", Nulovacio, "Por favor escribe un nombre");
            if (nombre == default) return;
            tier = ComprobarValor<int>("\nDime el Tier: ", int.TryParse, "Por favor introduce un tier válido: ");
            if (tier == default) return;
            Console.WriteLine("Tier guardado!!");
            vida = ComprobarValor<int>("\nDime la vida: ", int.TryParse, "Por favor introduce una vida válida: ");
            if (vida == default) return;
            Console.WriteLine("Vida guardada!!");
            daño = ComprobarValor<int>("\nDime el daño: ", int.TryParse, "Por favor introduce un daño válido: ");
            if (daño == default) return;
            Console.WriteLine("Monstruo guardado!!");
            monstruos.Add(new Monstruo(nombre, tier, vida, daño));
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void ConsultarMonstruo(List<Monstruo> monstruos)
        {
            do
            {
                string? input;
                Console.WriteLine("\nDime el nombre del monstruo que te gustaria consultar (ESCRIBE 'X' PARA CANCELAR): ");
                Console.WriteLine("......................................................");
                Mostrar(monstruos);
                Console.WriteLine();
                input = (ComprobarValor<string>("", Nulovacio, "Por favor escribe un valor"));
                if (input == default) break;
                Monstruo? elegido = monstruos.FirstOrDefault(p => p.nombre.ToUpper() == input.ToUpper());
                if (elegido == null)
                {
                    Console.WriteLine("\nESE MONSTRUO NO EXISTE!!");
                }
                else
                {
                    elegido.Enseñar();
                    break;
                }
            } while (true);
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void Eliminar(ref List<Monstruo> monstruos)
        {
            do
            {
                string? nombre;
                Console.Write("\nDime el nombre del monstruo que te gustaria borrar (ESCRIBE 'X' PARA CANCELAR): ");
                nombre = ComprobarValor<string>("\nDime el nombre: ", Nulovacio, "Por favor escribe un nombre: ");
                if (nombre == default) break;
                int valor = monstruos.RemoveAll(p => p.nombre.ToUpper() == nombre.ToUpper());
                if (valor == 0) Console.WriteLine("No existe ningún monstruo llamado " + nombre);
                else
                {
                    Mostrar(monstruos);
                    Console.WriteLine("Monstruo eliminado");
                    break;
                }
            } while (true);
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void BuscarPorCaracteristica(List<Monstruo> monstruos)
        {
            bool encontrado;
            string? input;
            do
            {
                Console.Write("\nDime la característica que te gustaria consultar (ESCRIBE 'X' PARA CANCELAR): ");
                encontrado = false;
                input = ComprobarValor<string>("\nTIER | DAÑO | VIDA: ", Nulovacio, "Por favor escribe algo: ");
                if (input == default) break;
                input.ToLower();
                encontrado = input switch
                {
                    "tier" => BuscarTier(monstruos),
                    "vida" => BuscarVida(monstruos),
                    "daño" => BuscarDaño(monstruos),
                    _ => CaracteristicaInvalida(),
                };
                if (!encontrado && (input == "tier" || input == "vida" || input == "daño")) Console.WriteLine("No existe monstruo con ese valor de " + input);
            } while (!encontrado);
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static bool BuscarTier(List<Monstruo> monstruos)
        {
            int tier = ComprobarValor<int>("\nDime el Tier que te interesa: ", int.TryParse, "Por favor introduce un tier válido");
            if (tier == default) return true;
            return BuscarPorCaracteristica((p => p.Tier == tier), monstruos);
        }
        public static bool BuscarVida(List<Monstruo> monstruos)
        {
            Console.Write("\nDime la vida que te interesa: ");
            int vida = ComprobarValor<int>("\nDime la vida: ", int.TryParse, "Por favor introduce una vida válida: ");
            if (vida == default) return true;
            return BuscarPorCaracteristica((p => p.Vida <= vida), monstruos);
        }
        public static bool BuscarDaño(List<Monstruo> monstruos)
        {
            Console.Write("\nDime el daño que te interesa: ");
            int daño = ComprobarValor<int>("\nDime el daño: ", int.TryParse, "Por favor introduce un daño válido: ");
            if (daño == default) return true;
            return BuscarPorCaracteristica((p => p.Daño <= daño), monstruos);

        }
        public static bool CaracteristicaInvalida()
        {
            Console.WriteLine("\nESA CARACTERISTICA NO EXISTE: ");
            return false;
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public delegate bool Parse<T>(string? input, out T result);
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static bool Nulovacio(string? input, out string result)
        {
            if (string.IsNullOrWhiteSpace(input)) 
            { 
                result = string.Empty;
                return false; 
            }
            else
            {
                result = input;
                return true;
            }
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static T? ComprobarValor<T>(string pregunta, Parse<T> Parse, string error)
        {
            do
            {
                T resultado;
                Console.Write(pregunta);
                string? input = Console.ReadLine();
                if (input?.ToUpper() == "X") return default;
                if (Parse(input, out resultado)) return resultado;
                Console.Write(error);
            } while (true);
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static bool BuscarPorCaracteristica(Func<Monstruo,bool> Comparacion, List<Monstruo> monstruos)
        {
            var TiersLista = monstruos.Where(Comparacion).ToList();
            if (TiersLista.Count != 0)
            {
                Mostrar(TiersLista);
                return true;
            }
            return false;
        }
    
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void Mostrar(List<Monstruo> lista)
        {
            int contador = 0;
            foreach (Monstruo monstruo in lista)
            {
                ++contador;
                if (contador % 2 == 0) Console.WriteLine(monstruo.nombre);
                else Console.Write($"{monstruo.nombre,-13}");
            }
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static void Guardar<T>(T objeto, string ruta)
        {
            string json;
            json = JsonSerializer.Serialize(objeto);
            File.WriteAllText(ruta, json);
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static T Importar<T>(string ruta) 
        {
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(File.ReadAllText(ruta), opciones)!;
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public class Monstruo
    {
        public string nombre { get; set; }
        private int tier;
        public int Tier
        {
            get { return tier; }
            set
            {
                if (value > 3) tier = 3;
                else if (value < 1) tier = 1;
                else tier = value;
            }
        }
        private int vida;
        public int Vida
        {
            get { return vida; }
            set
            {
                if (value > 500) vida = 500;
                else if (value < 1) vida = 1;
                else vida = value;
            }
        }
        private int daño;
        public int Daño
        {
            get { return daño; }
            set
            {
                if (value > 100) daño = 100;
                else if (value < 0) daño = 0;
                else daño = value;
            }
        }
        public Monstruo() { }
        public Monstruo(string nombre, int tier, int vida, int daño)
        {
            this.nombre = nombre;
            this.Tier = tier;
            this.Vida = vida;
            this.Daño = daño;
        }
        public void Enseñar()
        {
            Console.WriteLine($"\nNombre: {nombre}\nTier: {Tier}\nTiene {Vida} puntos de vida.\nHace {Daño} puntos de daño.");
        }
    }
}
