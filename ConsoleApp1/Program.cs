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
                respuesta = Input<string>("", Nulovacio, "Por favor escribe algo: ");
                if (respuesta != "1" && respuesta != "2" && respuesta != "3" && respuesta != "4") Console.WriteLine("\nNo has introducido un valor apto...");
                else
                {
                    switch (respuesta)
                    {
                        case "1":
                            Console.Write("\nESCRIBE 'X' EN CUALQUIER MOMENTO PARA CANCELAR");
                            nombre = Input<string>("\nDime el nombre: ", Nulovacio, "Por favor escribe un nombre");
                            if (nombre == default) break;
                            tier = Input<int>("\nDime el Tier: ", int.TryParse, "Por favor introduce un tier válido: ");
                            if (tier == default) break;
                            Console.WriteLine("Tier guardado!!");
                            vida = Input<int>("\nDime la vida: ", int.TryParse, "Por favor introduce una vida válida: ");
                            if (vida == default) break;
                            Console.WriteLine("Vida guardada!!");
                            daño = Input<int>("\nDime el daño: ", int.TryParse, "Por favor introduce un daño válido: ");
                            if (daño == default) break;
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
                                input = (Input<string>("", Nulovacio, "Por favor escribe un valor")).ToUpper();
                                Console.WriteLine("Input vale " + input);
                                if (input == default) break;
                                elegido = monstruos.FirstOrDefault(p => p.nombre.ToUpper() == input);
                                if (elegido == null)
                                {
                                    Console.WriteLine("\nESE MONSTRUO NO EXISTE!!");
                                }
                                else
                                {
                                    elegido.Enseñar();
                                }
                            } while (elegido == null);
                            break;
                        case "3":
                            do
                            {
                                Console.Write("\nDime el nombre del monstruo que te gustaria borrar (ESCRIBE 'X' PARA CANCELAR): ");
                                nombre = Input<string>("\nDime el nombre: ", Nulovacio, "Por favor escribe un nombre: ");
                                if (nombre == default) break;
                                int valor = monstruos.RemoveAll(p => p.nombre.ToUpper() == nombre);
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
                            Console.Write("\nDime la característica que te gustaria consultar (ESCRIBE 'X' PARA CANCELAR): ");
                            Console.Write("\nTIER | DAÑO | VIDA: ");
                            do
                            {
                                repetir = true;
                                encontrado = false;
                                input = Input<string>("", Nulovacio, "Por favor escribe algo: ");
                                switch (input)
                                {
                                    case null:
                                        encontrado = true;
                                        break;
                                    case "tier":
                                        Console.Write("\nDime que tier te interesa: ");
                                        tier = Input<int>("\nDime el Tier: ", int.TryParse, "Por favor introduce un tier válido: ");
                                        if (tier == default) { encontrado = true; break; }
                                        var TiersLista = monstruos.Where(p => p.Tier == tier).ToList();
                                        if (TiersLista.Count != 0)
                                        {
                                            encontrado = true;
                                            Mostrar(TiersLista);
                                        }
                                        break;
                                    case "vida":
                                        Console.Write("\nDime la vida que te interesa: ");
                                        vida = Input<int>("\nDime la vida: ", int.TryParse, "Por favor introduce una vida válida: ");
                                        if (vida == default) { encontrado = true; break; }
                                        var VidaLista = monstruos.Where(p => p.Vida <= vida).ToList();
                                        if (VidaLista.Count != 0)
                                        {
                                            encontrado = true;
                                            Mostrar(VidaLista);
                                        }
                                        break;
                                    case "daño":
                                        Console.Write("\nDime el daño que te interesa: ");
                                        daño = Input<int>("\nDime el daño: ", int.TryParse, "Por favor introduce un daño válido: ");
                                        if (daño == default) { encontrado = true; break; }
                                        var DañoLista = monstruos.Where(p => p.Daño <= daño).ToList();
                                        if (DañoLista.Count != 0)
                                        {
                                            encontrado = true;
                                            Mostrar(DañoLista);
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
            Console.WriteLine("\n\n             *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.WriteLine("\n               GRACIAS POR USAR LA BASE DE DATOS DE R.E.P.O");
            Console.WriteLine("\n               *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*");
            Console.ReadKey();
        }
        public delegate bool Parse<T>(string? input, out T result);

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
        public static T? Input<T>(string pregunta, Parse<T> Parse, string error)
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
