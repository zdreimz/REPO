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
                            Console.Write("\nDime el nombre: ");
                            nombre = es_valido("nombre");
                            if (nombre.ToUpper() == "X") break;
                            Console.Write("\nDime el Tier: ");
                            input = es_valido("tier");
                            if (input.ToUpper() == "X") break;
                            tier = Convert.ToInt32(input);
                            Console.WriteLine("Tier guardado!!");
                            Console.Write("\nDime la vida: ");
                            input = es_valido("vida");
                            if (input.ToUpper() == "X") break;
                            vida = Convert.ToInt32(input);
                            Console.WriteLine("Vida guardada!!");
                            Console.Write("\nDime el daño: ");
                            input = es_valido("daño");
                            if (input.ToUpper() == "X") break;
                            daño = Convert.ToInt32(input);
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
                                    Console.WriteLine($"\nNombre: {elegido.nombre}\nTier: {elegido.tier}\nTiene {elegido.vida} puntos de vida.\nHace {elegido.daño} puntos de daño.");
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
                                            if (monstruillo.tier == tier)
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
                                            if (monstruillo.vida == vida)
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
                                            if (monstruillo.daño == daño)
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
        private int Tier;
        public int tier
        {
            get { return Tier; }
            set
            {
                if (value > 3) Tier = 3;
                else if (value < 1) Tier = 1;
                else Tier = value;
            }
        }
        private int Vida;
        public int vida
        {
            get { return Vida; }
            set
            {
                if (value > 500) Vida = 500;
                else if (value < 1) Vida = 1;
                else Vida = value;
            }
        }
        private int Daño;
        public int daño
        {
            get { return Daño; }
            set
            {
                if (value > 100) Daño = 100;
                else if (value < 0) Daño = 0;
                else Daño = value;
            }
        }
        public Monstruo(string nombre, int tier, int vida, int daño)
        {
            this.nombre = nombre;
            this.tier = tier;
            this.vida = vida;
            this.daño = daño;
        }
    }
}
