using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Channels;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Dynamic;
using System.Xml;

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
            string nombre;
            string input;
            string respuesta;
            const string archivo = "monstruos.json";
            string ruta = Path.Combine(AppContext.BaseDirectory, archivo);
            List<Monstruo> monstruos = Importar<List<Monstruo>>(ruta);
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.WriteLine("\n                           MONSTRUOS DEL R.E.P.O");
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            do {
                repetir = true;
                Console.Write("\n\nQue quieres hacer? Escribe el número: \n\n1.Guardar un monstruo nuevo | 2.Consultar Monstruo | 3.Borrar Monstruo | 4.Buscar por característica: ");
                respuesta = es_valido("");
                if (respuesta != "1" && respuesta != "2" && respuesta != "3" && respuesta != "4") Console.WriteLine("\nNo has introducido un valor apto...");
                else
                {
                    switch (respuesta)
                    {
                        case "1":
                            Console.Write("\nESCRIBE 'X' EN CUALQUIER MOMENTO PARA CANCELAR");
                            nombre = Input<string>("\nDime el nombre: ", (string? a, out string result) => { if (string.IsNullOrWhiteSpace(a)) { result = string.Empty; return false; } else { result = a; return true; } }, "Por favor escribe un nombre");
                            if (nombre == null) break;
                            tier = Input<int>("\nDime el Tier: ", int.TryParse, "Por favor introduce un tier válido: ");
                            if (tier == 0) break;
                            Console.WriteLine("Tier guardado!!");
                            vida = Input<int>("\nDime la vida: ", int.TryParse, "Por favor introduce una vida válida: ");
                            if (vida == 0) break;
                            Console.WriteLine("Vida guardada!!");
                            daño = Input<int>("\nDime el daño: ", int.TryParse, "Por favor introduce un daño válido: ");
                            if (daño == 0) break;
                            Console.WriteLine("Monstruo guardado!!");
                            monstruos.Add(new Monstruo(nombre, tier, vida, daño));
                            Guardar(monstruos, ruta);
                            break;
                        case "2":
                            Monstruo elegido;
                            do
                            {
                                Console.WriteLine("\nDime el nombre del monstruo que te gustaria consultar (ESCRIBE 'X' PARA CANCELAR): ");
                                Console.WriteLine("......................................................");
                                Mostrar(monstruos);
                                Console.WriteLine();

                                input = es_valido("").ToUpper();
                                if (input.ToUpper() == "X") break;
                                elegido = monstruos.FirstOrDefault(p => p.nombre.ToUpper() == input)!;
                                if (elegido == null)
                                {
                                    Console.WriteLine("\nESE MONSTRUO NO EXISTE!!");
                                }
                                else
                                {
                                    Console.WriteLine($"\nNombre: {elegido.nombre}\nTier: {elegido.Tier}\nTiene {elegido.Vida} puntos de vida.\nHace {elegido.Daño} puntos de daño.");
                                }
                            } while (elegido == null);
                            break;
                        case "3":
                            do
                            {
                                Console.Write("\nDime el nombre del monstruo que te gustaria borrar (ESCRIBE 'X' PARA CANCELAR): ");
                                nombre = es_valido("nombre").ToUpper();
                                if (nombre == "X") break;
                                int valor = monstruos.RemoveAll(p => p.nombre.ToUpper() == nombre);
                                if (valor == 0) Console.WriteLine("No existe ningún monstruo llamado " + nombre);
                                else
                                {
                                    Guardar(monstruos, ruta);
                                    Mostrar(monstruos);
                                    Console.WriteLine("Monstruo eliminado");
                                    repetir = false;
                                } 
                            } while (repetir);
                            break;
                        case "4":
                            Console.Write("\nDime la característica que te gustaria consultar (ESCRIBE 'X' PARA CANCELAR): ");
                            Console.Write("\nTIER | DAÑO | VIDA: ");
                            do
                            {
                                repetir = true;
                                encontrado = false;
                                input = es_valido("").ToLower();
                                switch (input)
                                {
                                    case "x":
                                        encontrado = true;
                                        break;
                                    case "tier":
                                        Console.Write("\nDime que tier te interesa: ");
                                        tier = Convert.ToInt32(es_valido("tier"));
                                        foreach (Monstruo monstruillo in monstruos)
                                        {
                                            if (monstruillo.Tier == tier)
                                            {
                                                Console.Write(monstruillo.nombre + " || "); encontrado = true;
                                            }
                                        }
                                        break;
                                    case "vida":
                                        Console.Write("\nDime la vida que te interesa: ");
                                        vida = Convert.ToInt32(es_valido("vida"));
                                        foreach (Monstruo monstruillo in monstruos)
                                        {
                                            if (monstruillo.Vida == vida)
                                            {
                                                Console.Write(monstruillo.nombre + " || "); encontrado = true;
                                            }
                                        }
                                        break;
                                    case "daño":
                                        Console.Write("\nDime el daño que te interesa: ");
                                        daño = Convert.ToInt32(es_valido("daño"));
                                        foreach (Monstruo monstruillo in monstruos)
                                        {
                                            if (monstruillo.Daño == daño)
                                            {
                                                Console.Write(monstruillo.nombre + " || "); encontrado = true;
                                            }
                                        }
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
            Console.WriteLine("\n\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.WriteLine("\n               GRACIAS POR USAR LA BASE DE DATOS DE R.E.P.O");
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.ReadKey();
        }
        public delegate bool Parse<T>(string? input, out T result);
        public static T? Input<T>(string pregunta, Parse<T> Parse, string error)
        {
            do
            {
                T resultado;
                Console.WriteLine(pregunta);
                string? input = Console.ReadLine();
                if (input?.ToUpper() == "X") return default;
                if (Parse(input, out resultado)) return resultado;
                Console.WriteLine(error);
            } while (true);
        }
        
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string es_valido(string cosa)
        {
            bool repetir = true;
            string entrada;
            do
            {
                entrada = Console.ReadLine()!;
                if (String.IsNullOrEmpty(entrada)) Console.Write("Por favor introduce un valor: ");
                else if (entrada.ToUpper() == "X") return entrada;
                else
                {
                    switch (cosa)
                    {
                        case "nombre":
                            if (int.TryParse(entrada, out _)) Console.Write("Por favor introduce un nombre válido: ");
                            else repetir = false;
                            break;
                        case "tier":
                            if (!int.TryParse(entrada, out _) || (entrada != "1" && entrada != "2" && entrada != "3")) Console.Write("Por favor introduce un tier válido: ");
                            else repetir = false;
                            break;
                        case "vida":
                            if (!int.TryParse(entrada, out _)) Console.Write("Por favor introduce una vida válida: ");
                            else repetir = false;
                            break;
                        case "daño":
                            if (!int.TryParse(entrada, out _)) Console.Write("Por favor introduce un daño válido: ");
                            else repetir = false;
                            break;
                        default:
                            repetir = false;
                            break;
                    }

                } 
            } while (repetir);
            return entrada;
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void Mostrar(List<Monstruo> lista)
        {
            foreach (Monstruo monstruo in lista)
            {
                Console.WriteLine(monstruo.nombre);
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
            return JsonSerializer.Deserialize<T>(File.ReadAllText(ruta))!;
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }
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
        public Monstruo(string nombre, int tier, int vida, int daño)
        {
            this.nombre = nombre;
            this.Tier = tier;
            this.Vida = vida;
            this.Daño = daño;
        }
    }
}
