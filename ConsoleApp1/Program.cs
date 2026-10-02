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
            int tier;
            int vida;
            int daño;
            bool repetir;
            bool encontrado;
            string volver;
            string? nombre;
            string? input;
            string? respuesta;
            const string archivo = "monstruos.json";
            string ruta = Path.Combine(AppContext.BaseDirectory, archivo);
            List<Monstruo> monstruos = Importar<List<Monstruo>>(ruta);
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.WriteLine("\n                           MONSTRUOS DEL R.E.P.O");
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            do {
                repetir = true;
                Console.Write("\n\nQue quieres hacer? Escribe el número: \n\n1.Guardar un monstruo nuevo | 2.Consultar Monstruo | 3.Borrar Monstruo | 4.Buscar por característica: ");
                respuesta = ComprobarValor<string>("", Nulovacio, "Por favor escribe algo: ");
                if (respuesta != "1" && respuesta != "2" && respuesta != "3" && respuesta != "4") Console.WriteLine("\nNo has introducido un valor apto...");
                else
                {
                    switch (respuesta)
                    {
                        case "1":
                            Monstruo nuevo = AñadirMonstruo();
                            if (nuevo != null)
                            {
                                monstruos.Add(nuevo);
                                Guardar(monstruos, ruta);
                            }
                            break;
                        case "2":
                            ConsultarMonstruo(monstruos);
                            break;
                        case "3":
                            do
                            {
                                Console.Write("\nDime el nombre del monstruo que te gustaria borrar (ESCRIBE 'X' PARA CANCELAR): ");
                                nombre = ComprobarValor<string>("\nDime el nombre: ", Nulovacio, "Por favor escribe un nombre: ");
                                if (nombre == default) break;
                                int valor = monstruos.RemoveAll(p => p.nombre.ToUpper() == nombre.ToUpper());
                                if (valor == 0) Console.WriteLine("No existe ningún monstruo llamado " + nombre);
                                else
                                {
                                    Guardar(monstruos, ruta);
                                    Mostrar(monstruos);
                                    Console.WriteLine("Monstruo eliminado");
                                    break;
                                } 
                            } while (true);
                            break;
                        case "4":
                            do 
                            {
                                Console.Write("\nDime la característica que te gustaria consultar (ESCRIBE 'X' PARA CANCELAR): ");
                                repetir = true;
                                encontrado = false;
                                input = ComprobarValor<string>("\nTIER | DAÑO | VIDA: ", Nulovacio, "Por favor escribe algo: ");
                                switch (input)
                                {
                                    case null:
                                        encontrado = true;
                                        break;
                                    case "tier":
                                        tier = ComprobarValor<int>("\nDime el Tier que te interesa: ", int.TryParse, "Por favor introduce un tier válido");
                                        if (tier == default) { encontrado = true; break; }
                                        encontrado = BuscarPorCaracteristica((p => p.Tier == tier), monstruos); 
                                        break;
                                    case "vida":
                                        Console.Write("\nDime la vida que te interesa: ");
                                        vida = ComprobarValor<int>("\nDime la vida: ", int.TryParse, "Por favor introduce una vida válida: ");
                                        if (vida == default) { encontrado = true; break; }
                                        encontrado = BuscarPorCaracteristica((p => p.Vida <= vida), monstruos);
                                        break;
                                    case "daño":
                                        Console.Write("\nDime el daño que te interesa: ");
                                        daño = ComprobarValor<int>("\nDime el daño: ", int.TryParse, "Por favor introduce un daño válido: ");
                                        if (daño == default) { encontrado = true; break; }
                                        encontrado = BuscarPorCaracteristica((p => p.Daño <= daño), monstruos);
                                        break;
                                    default:
                                        Console.WriteLine("\nESA CARACTERISTICA NO EXISTE: ");
                                        break;
                                }
                                if (encontrado == false) Console.WriteLine("No existe monstruo con ese valor de " + input);
                            } while (repetir && !encontrado);
                            break;

                    }
                    repetir = true;
                    do
                    {
                        Console.Write("\n¿Te gustaria realizar otra operación? SI/NO: ");
                        volver = Console.ReadLine()!.ToUpper()!;
                    } while (volver != "SI" && volver != "NO");
                    if (volver == "NO") repetir = false;
                }
            } while (repetir);
            Console.WriteLine("\n\n              *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.WriteLine("\n               GRACIAS POR USAR LA BASE DE DATOS DE R.E.P.O");
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.ReadKey();
        }
        public static Monstruo AñadirMonstruo()
        {
            string? nombre;
            int tier;
            int vida;
            int daño;
            Console.Write("\nESCRIBE 'X' EN CUALQUIER MOMENTO PARA CANCELAR");
            nombre = ComprobarValor<string>("\nDime el nombre: ", Nulovacio, "Por favor escribe un nombre");
            if (nombre == default) return default;
            tier = ComprobarValor<int>("\nDime el Tier: ", int.TryParse, "Por favor introduce un tier válido: ");
            if (tier == default) return default;
            Console.WriteLine("Tier guardado!!");
            vida = ComprobarValor<int>("\nDime la vida: ", int.TryParse, "Por favor introduce una vida válida: ");
            if (vida == default) return default;
            Console.WriteLine("Vida guardada!!");
            daño = ComprobarValor<int>("\nDime el daño: ", int.TryParse, "Por favor introduce un daño válido: ");
            if (daño == default) return default;
            Console.WriteLine("Monstruo guardado!!");
            return new Monstruo(nombre, tier, vida, daño);
        }
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
                Monstruo elegido = monstruos.FirstOrDefault(p => p.nombre.ToUpper() == input.ToUpper());
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
