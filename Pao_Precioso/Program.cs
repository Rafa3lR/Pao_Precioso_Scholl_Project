using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;

internal class Program
{
    private static int posic = 0, xMenu = 1, xStock = 1, xSales = 1, edit = 0, scrollStock = 0, scrollSales = 0, ident = 1, xReports = 0, select = 0, limitReportUp = 0, limitReportDown = 0;
    private const int almostExpiring = 15;
    private static int limitUpStock = 0, limitDownStock = 0, limitUpSales = 0, limitDownSales = 0, filter = 0, add = 0, delete = 0, maxProd = 1, scrollReport = 0;
    private static string reportFinal = "", filterTxt = "All products";
    private static float totalSale = 0, totalReports = 0;
    private static DateTime today = DateTime.Now;
    private static TimeSpan intervalChangeColor;


    private static void Main(string[] args)
    {
        //Verify if .txt exists and create if it doesn't (Verifica se o .txt existe e cria se não existir)
        StreamWriter ID1 = new StreamWriter("ID.txt", true);
        ID1.Close();
        StreamWriter productID1 = new StreamWriter("productID.txt", true);
        productID1.Close();
        StreamWriter name1 = new StreamWriter("pruductName.txt", true);
        name1.Close();
        StreamWriter price1 = new StreamWriter("pruductPrice.txt", true);
        price1.Close();
        StreamWriter quant1 = new StreamWriter("pruductQuant.txt", true);
        quant1.Close();
        StreamWriter expirationDate1 = new StreamWriter("productExpirationDate.txt", true);
        expirationDate1.Close();
        StreamWriter reportSales1 = new StreamWriter("reportSales.txt", true);
        reportSales1.Close();
        StreamWriter totalReports1 = new StreamWriter("totalReports.txt", true);
        totalReports1.Close();
        StreamWriter allReports1 = new StreamWriter("allReports.txt", true);
        allReports1.Close();
        
        if (add == 0 && delete == 0)
        {
            using (StreamReader sr = new StreamReader("pruductPrice.txt"))
            {
                string price;
                while ((price = sr.ReadLine()) != null)
                {
                    maxProd++;
                }
            }
        }

        Stock[] products = new Stock[maxProd + 1];
        float[] quantSale = new float[maxProd + 1];

        using (StreamReader sr = new StreamReader("pruductName.txt"))
        {
            posic = 0;
            string name;
            while ((name = sr.ReadLine()) != null)
            {
                products[posic].name = name;
                posic++;
            }
            posic = 0;
        }
        using (StreamReader sr = new StreamReader("pruductPrice.txt"))
        {
            string price;
            while ((price = sr.ReadLine()) != null)
            {
                products[posic].price = Convert.ToSingle(price);
                posic++;
            }
            posic = 0;
        }
        using (StreamReader sr = new StreamReader("pruductQuant.txt"))
        {
            string quant;
            while ((quant = sr.ReadLine()) != null)
            {
                products[posic].quant = Convert.ToSingle(quant);
                posic++;
            }
            posic = 0;
        }
        using (StreamReader sr = new StreamReader("productExpirationDate.txt"))
        {
            string expirationDate;
            while ((expirationDate = sr.ReadLine()) != null)
            {
                products[posic].expirationDate = Convert.ToDateTime(expirationDate);
                posic++;
            }
            posic = 0;
        }
        using (StreamReader sr = new StreamReader("productID.txt"))
        {
            string productID;
            while ((productID = sr.ReadLine()) != null)
            {
                products[posic].iD = Convert.ToInt32(productID);
                posic++;
            }
        }
        using (StreamReader sr = new StreamReader("ID.txt"))
        {
            string id;
            while((id = sr.ReadLine()) != null)
            {
                ident = Convert.ToInt32(id);
            }
        }
        

        if (add == 1)
        {
            add = 0;
            StockMenu(ref products, args, ref quantSale);
        }
        if (delete == 1)
        {
            delete = 0;
            StockMenu(ref products, args, ref quantSale);
        }

        Menu(ref products, args, ref quantSale);
    }

    //Data struct for the products (Estrutura de dados para os produtos)
    struct Stock
    {
        public int iD;
        public string name;
        public float quant;
        public float price;
        public DateTime expirationDate;
    }

    //Write something in a specified location in the console (Escreve algo em uma área específica do console)
    private static void WriteAT(string a, int x, int y)
    { 
        try
        {
            Console.SetCursorPosition(x, y);   //Sets the position where Console.Write will start printing (configura a posição a partir do qual o Console.Write vai começar a "printar")
            Console.Write(a);   //Writes the string (Escreve a string)
        }
        catch { }
    }
    




    //Writes a simple menu and a way to interact with the keyboard arrows (Escreve um menu simples e uma forma de interagir com as teclas de seta do teclado)
    private static void Menu(ref Stock[] products, string[] args, ref float[] quantSale)
    {
        string option = "";

        while (option != "Enter")
        {
            Console.Clear();
            Console.WriteLine("\n                 Welcome!");
            Console.WriteLine("      (use arrow keys for navigation)");
            Console.WriteLine("\n    Sales     Stock    Reports     Exit");
            
            //Check if the arrow keys are pressed (Observa se as setinhas foram apertadas)
            if (option == "RightArrow")
            {
                if (xMenu < 31)
                {
                    xMenu += 10;
                }
                else if (xMenu == 31)
                {
                    xMenu = 1;
                }
            }

            if (option == "LeftArrow")
            {
                if (xMenu > 1)
                {
                    xMenu -= 10;
                }
                else if (xMenu == 1)
                {
                    xMenu = 31;
                }
            }

            //Selection "pointer" ("Selecionador")
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            WriteAT("+---------+", xMenu, 5);

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            WriteAT("+-----------------------------------------+", 0, 0);
            Console.Write("\n|\n|\n\n|\n|");
            WriteAT("|", 42, 1); WriteAT("|", 42, 4); WriteAT("|", 42, 2);
            Console.Write("\n|-----------------------------------------|");
            WriteAT("|", 42, 5);
            Console.Write("\n+-----------------------------------------+");
            Console.ResetColor();

            option = Convert.ToString(Console.ReadKey().Key);
        }

        //Defines which optin was selected (Define qual opção foi selecionada)
        if (xMenu == 1)
        {
            SalesMenu(ref products, args, ref quantSale);
        }
        else if(xMenu == 11)
        {
            StockMenu(ref products, args, ref quantSale);
        }
        else if(xMenu == 21)
        {
            ReportSales();
        }
        else if(xMenu == 31)
        {
            EndProgram();
        }

        //Calls itself to set a loop (Chama a si mesmo para definir um loop)
        Menu(ref products, args, ref quantSale);
    }





    //Sales Menu (Menu de vendas)
    private static void SalesMenu(ref Stock[] products, string[] args, ref float[] quantSale)
    {
        string option = "";
        int y = 9 , yy = -1; 

        while (option != "Enter")
        {
            y = 9;
            Console.Clear();
            Console.WriteLine("\n                                    Sales");

            WriteProductsSalesScreen(products, ref y, ref yy, ref quantSale);

            SelectOptionSalesMenu(ref option);
        }

        //Defines which optin was selected (Define qual opção foi selecionada)
        if (xSales == 1)
        {
            AddProductSales(ref products, args, ref quantSale);
        }
        else if (xSales == 17)
        {
            ConsultStock(products, ref y, ref yy);
        }
        else if (xSales == 33)
        {
            DeleteProdSales(ref products, args, ref quantSale);
        }
        else if (xSales == 49)
        {
            EndSale(products, args, ref quantSale);
        }
        else if (xSales == 65)
        {
            CancelSale(ref products, args, ref quantSale);
        }

        SalesMenu(ref products, args, ref quantSale);
    }

    private static void AddProductSales(ref Stock[] products, string[] args, ref float[] quantSale)
    {
        int opcao = maxProd + 1;
        float quant = 0;
        string confirm = "";

        Console.Clear();
        Console.WriteLine("Add. product sale\n");
        Console.Write("Product cod.: ");
        try
        {
            opcao = Convert.ToInt32(Console.ReadLine());
            opcao = BinSearch(products, opcao, maxProd);

            if (opcao >= 0)
            {
                if (products[opcao].quant > 0)
                {
                    intervalChangeColor = products[opcao].expirationDate - today;

                    if (intervalChangeColor.Days >= 0)
                    {
                        Console.Write($"|  {products[opcao].name} - {products[opcao].quant}  |  Confirm? (s/n): ");
                        confirm = Console.ReadLine();

                        if (confirm == "s" || confirm == "S")
                        {
                            Console.Write("Quant.: ");
                            try
                            {
                                quant = Convert.ToSingle(Console.ReadLine());
                            }
                            catch { }

                            if (products[opcao].quant >= quant)
                            {
                                products[opcao].quant -= quant;
                                quantSale[opcao] += quant;
                                totalSale += quant * products[opcao].price;

                            }
                            else
                            {
                                Console.Write("Not enough balance! ");
                                Console.ReadKey();
                                SalesMenu(ref products, args, ref quantSale);
                            }
                        }
                        else
                        {
                            SalesMenu(ref products, args, ref quantSale);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n( " + products[opcao].name + " )\n");
                        Console.ResetColor();
                        Console.Write("Expired product!");
                        Console.ReadKey();
                        SalesMenu(ref products, args, ref quantSale);
                    }
                }
                else
                {
                    Console.Write("Invalid product! ");
                    Console.ReadKey();
                    SalesMenu(ref products, args, ref quantSale);
                }
            }
            else
            {
                Console.Write("Invalid product! ");
                Console.ReadKey();
                SalesMenu(ref products, args, ref quantSale);
            }
        }
        catch { }

        SalesMenu(ref products, args, ref quantSale);
    }

    private static void DeleteProdSales(ref Stock[] products, string[] args, ref float[] quantSale)
    {
        int opcao = maxProd + 1;
        string confirm = "";

        Console.Clear();
        Console.WriteLine("Delete product sale\n");
        Console.Write("Product cod.: ");
        try
        {
            opcao = Convert.ToInt32(Console.ReadLine());
            opcao = BinSearch(products, opcao, maxProd);

            if (opcao >= 0)
            {
                if (quantSale[opcao] > 0)
                {
                    Console.WriteLine($"\n( {products[opcao].name} )\n");
                    Console.Write("confirm? (s/n): ");
                    confirm = Console.ReadLine();

                    if (confirm == "s" || confirm == "S")
                    {
                        products[opcao].quant += quantSale[opcao];
                        totalSale -= quantSale[opcao] * products[opcao].price;
                        quantSale[opcao] = 0;
                    }
                }
                else
                {
                    Console.Write("Invalid product! ");
                    Console.ReadKey();
                    SalesMenu(ref products, args, ref quantSale);
                }
            }
            else
            {
                Console.Write("Invalid product! ");
                Console.ReadKey();
                SalesMenu(ref products, args, ref quantSale);
            }
        }
        catch { }

        SalesMenu(ref products, args, ref quantSale);
    }

    private static void EndSale(Stock[] products, string[] args, ref float[] quantSale)
    {
        string confirm = "";

        Console.Clear();
        Console.Write("Confirm? (s/n)  ");
        confirm = Console.ReadLine();
        if (confirm == "s" || confirm == "S")
        {
            for (int i = 0; i < maxProd; i++)
            {
                if (quantSale[i] > 0)
                {
                    using (StreamWriter sw = new StreamWriter("reportSales.txt", true))
                    {
                        sw.WriteLine($"(Cod.: {products[i].iD}) {products[i].name} - {quantSale[i]} * ${products[i].price} = ${products[i].price * quantSale[i]}");
                    }

                    quantSale[i] = 0;
                }
            }

            totalReports += totalSale;

            using (StreamWriter sw = new StreamWriter("totalReports.txt", false))
            {
                sw.Write(totalReports);
            }

            StreamWriter sw1 = new StreamWriter("pruductQuant.txt", false);
            sw1.Close();

            for (int i = 0; i < maxProd; i++)
            {
                if (products[i].price > 0)
                {
                    using (StreamWriter sw = new StreamWriter("pruductQuant.txt", true))
                    {
                        sw.WriteLine(products[i].quant);
                    }
                }
            }

            totalSale = 0;

            Menu(ref products, args, ref quantSale);
        }
        else
        {
            SalesMenu(ref products, args, ref quantSale);
        }
    }

    private static void CancelSale(ref Stock[] products, string[] args, ref float[] quantSale)
    {
        string confirm = "";

        Console.Clear();
        Console.Write("Confirm? (s/n)  ");
        confirm = Console.ReadLine();
        if (confirm == "s" || confirm == "S")
        {
            for(int i = 0; i < maxProd; i++)
            {
                products[i].quant += quantSale[i];
                quantSale[i] = 0;
            }

            Menu(ref products, args, ref quantSale);
        }
        else
        {
            SalesMenu(ref products, args, ref quantSale);
        }
    }

    private static void WriteProductsSalesScreen(Stock[] products, ref int y, ref int yy, ref float[] quantSale)
    {
        WriteAT($"Total: {totalSale}", 1, 5);
        WriteAT("   Cod.     Description                                   Quant.        Price     ", 0, 7);
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        WriteAT("|---------------------------------------------------------------------------------|", 0, 6);
        WriteAT("|", 0, 7); WriteAT("|", 9, 7); WriteAT("|", 56, 7); WriteAT("|", 70, 7); WriteAT("|", 82, 7);
        Console.WriteLine("\n|---------------------------------------------------------------------------------|");
        
        WriteAT("+---------------------------------------------------------------------------------+", 0, 0);
        WriteAT("|", 0, 1); WriteAT("|", 82, 1);
        WriteAT("|---------------------------------------------------------------------------------|", 0, 2);
        WriteAT("|", 0, 3); WriteAT("|", 82, 3);
        WriteAT("|", 0, 4); WriteAT("|", 82, 4);
        WriteAT("|", 0, 5); WriteAT("|", 82, 5);
        Console.ResetColor();

        for (int i = 0; i < maxProd; i++)
        {
            if (quantSale[i] > 0)
            {
                if ((y + scrollSales) >= 9 && (y + scrollSales) < 29)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    WriteAT("|        |                                              |             |           |", 0, y + scrollSales);
                    Console.ResetColor();
                    WriteAT($"{products[i].iD}", 4, y + scrollSales); WriteAT($"{products[i].name}", 12, y + scrollSales); WriteAT($"{quantSale[i]}", 58, y + scrollSales); WriteAT($"${products[i].price * quantSale[i]}", 72, y + scrollSales);
                }
                y++;
            }
        }

        if (y + scrollSales > 9 && y + scrollSales < 29)
        {
            yy = y + scrollSales;

        }
        else if (y + scrollSales >= 29)
        {
            yy = 29;
        }

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        WriteAT("+---------------------------------------------------------------------------------+", 0, yy);

        if (scrollSales < 0)
        {
            WriteAT("|                                   ^^ SCROLL UP TO SEE MORE ^^                     |", 0, 9);
            limitUpSales = 1;
        }
        else
        {
            limitUpSales = 0;
        }
        if ((y + scrollSales) >= 30)
        {
            WriteAT("|                                   vv SCROLL DOWN TO SEE MORE vv                   |", 0, 28);
            Console.Write("\n+---------------------------------------------------------------------------------+");
            limitDownSales = 1;
        }
        else
        {
            limitDownSales = 0;
        }
        Console.ResetColor();
    }

    private static void ConsultStock(Stock[] products, ref int y, ref int yy)
    {
        string option = "";
        int scrollConsultStock = 0, limitUpConsultStock = 0, limitDownConsultStock = 0;

        while (option != "Enter")
        {
            y = 5;

            Console.Clear();
            Console.WriteLine("\n                                    Stock");
            WriteAT("   Cod.     Description                Price       Quant.       Expiration     ", 0, 3);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            WriteAT("+-----------------------------------------------------------------------------+", 0, 0);
            WriteAT("|", 0, 1); WriteAT("|", 78, 1);
            WriteAT("|-----------------------------------------------------------------------------|", 0, 2);
            WriteAT("|", 0, 3); WriteAT("|", 9, 3); WriteAT("|", 36, 3); WriteAT("|", 48, 3); WriteAT("|", 61, 3); WriteAT("|", 78, 3);
            Console.WriteLine("\n|-----------------------------------------------------------------------------|");
            Console.ResetColor();

            for (int i = 0; i < maxProd; i++)
            {
                if (products[i].price > 0)
                {
                    if ((y + scrollConsultStock) >= 5 && (y + scrollConsultStock) < 29)
                    {
                        intervalChangeColor = products[i].expirationDate - today;

                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        WriteAT("|        |                          |           |            |                |", 0, y + scrollConsultStock);
                        Console.ResetColor();
                        WriteAT($"{products[i].iD}", 4, y + scrollConsultStock); WriteAT($"{products[i].name}", 12, y + scrollConsultStock); WriteAT($"${products[i].price}", 39, y + scrollConsultStock); WriteAT($"{products[i].quant}", 51, y + scrollConsultStock);
                        if (intervalChangeColor.Days < 7 && intervalChangeColor.Days >= 0) { Console.ForegroundColor = ConsoleColor.Yellow; } else if (intervalChangeColor.Days < 0) { Console.ForegroundColor = ConsoleColor.Red; }
                        WriteAT(products[i].expirationDate.ToString("dd, MM, yyyy"), 64, y + scrollConsultStock); Console.ResetColor();
                    }
                    y++;
                }
            }

            if (y + scrollConsultStock > 5 && y + scrollConsultStock < 29)
            {
                yy = y + scrollConsultStock;

            }
            else if (y + scrollConsultStock >= 29)
            {
                yy = 29;
            }

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            WriteAT("+-----------------------------------------------------------------------------+", 0, yy);

            if (scrollConsultStock < 0)
            {
                WriteAT("|                         ^^ SCROLL UP TO SEE MORE ^^                         |", 0, 5);
                limitUpConsultStock = 1;
            }
            else
            {
                limitUpConsultStock = 0;
            }
            if ((y + scrollConsultStock) >= 30)
            {
                WriteAT("|                         vv SCROLL DOWN TO SEE MORE vv                       |", 0, 28);
                Console.Write("\n+-----------------------------------------------------------------------------+");
                limitDownConsultStock = 1;
            }
            else
            {
                limitDownConsultStock = 0;
            }
            Console.ResetColor();

            option = Convert.ToString(Console.ReadKey().Key);

            if (option == "UpArrow" && limitUpConsultStock == 1)
            {
                scrollConsultStock ++;
            }
            if (option == "DownArrow" && limitDownConsultStock == 1)
            {
                scrollConsultStock --;
            }
        }
    }

    private static void SelectOptionSalesMenu(ref string option)
    {
        WriteAT("   Add product    Consult Stock    Delete Prod.     End sale         Cancel  ", 1, 3);

        //Check if the arrow keys are pressed (Observa se as setinhas foram apertadas)
        if (option == "RightArrow")
        {
            if (xSales < 65)
            {
                xSales += 16;
            }
            else if (xSales == 65)
            {
                xSales = 1;
            }
        }

        if (option == "LeftArrow")
        {
            if (xSales > 1)
            {
                xSales -= 16;
            }
            else if (xSales == 1)
            {
                xSales = 65;
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        WriteAT("+---------------+", xSales, 4);
        Console.ResetColor();

        Console.SetCursorPosition(0, 1);

        option = Convert.ToString(Console.ReadKey().Key);

        //Check if the arrow keys are pressed (Observa se as setinhas foram apertadas)
        if (option == "UpArrow" && limitUpSales == 1)
        {
            scrollSales ++;
        }
        if (option == "DownArrow" && limitDownSales == 1)
        {
            scrollSales --;
        }
    }
    //End sales menu (Fim do menu de vendas)





    //Stock Menu (Menu de estoque)
    private static void StockMenu(ref Stock[] products, string[] args, ref float[] quantSale)
    {
        string option = "";
        int yy = -1;

        while (option != "Enter")
        {
            int y = 9;
            Console.Clear();
            Console.WriteLine($"\n                                     Stock - Today: {today.ToString("dd/MM/yyyy")}");

            WriteProductsStockScreen(products, ref y, ref yy);

            SelectOptionStockMenu(ref option);
        }

        if (xStock == 1 && edit == 0)
        {
            AddProductStock(ref products, args);
        }
        else if (xStock == 49 && edit == 0)
        {
            DeleteProd(ref products, args);
        }
        else if (xStock == 65 && edit == 0)
        {
            edit = 2;
            xStock = 1;
            StockMenu(ref products, args, ref quantSale);
        }
        else if (xStock == 81 && edit == 0)
        {
            Menu(ref products, args, ref quantSale);
        }
        else if (xStock == 1 && edit == 1)
        {
            ChangeQuantProd(ref products);
        }
        else if (xStock == 17 && edit == 0)
        {
            edit = 1;
            xStock = 1;
            StockMenu(ref products, args, ref quantSale);
        }
        else if (xStock == 17 && edit == 1)
        {
            ChangePrice(ref products);
        }
        else if (xStock == 33 && edit == 0)
        {
            ReturnProd(ref products);
        }
        else if (xStock == 33 && edit == 1)
        {
            ChangeDate(ref products);
        }
        else if (xStock == 49 && edit ==1)
        {
            ChangeName(ref products);
        }
        else if (xStock == 65 && edit == 1)
        {
            edit = 0;
            xStock = 17;
        }
        else if (xStock == 1 && edit == 2)
        {
            edit = 0;
            xStock = 65;
            filter = 0;
            filterTxt = "All products";
        }
        else if (xStock == 17 && edit == 2)
        {
            edit = 0;
            xStock = 65;
            filter = 1;
            filterTxt = "Expired prods.";
        }
        else if (xStock == 33 && edit == 2)
        {
            edit = 0;
            xStock = 65;
            filter = 2;
            filterTxt = "Almost expin.";
        }
        else if (xStock == 49 && edit == 2)
        {
            edit = 0;
            xStock = 65;
        }

        StockMenu(ref products, args, ref quantSale);
    }

    private static void AddProductStock(ref Stock[] products, string[] args)
    {
        if (posic < maxProd)
        {
            Console.Clear();
            Console.WriteLine("Add. Product\n");
            Console.Write("Product name: ");
            products[posic].name = Console.ReadLine();
            Console.Write("Price: ");
            try
            {
                products[posic].price = Convert.ToSingle(Console.ReadLine());
            }
            catch { }
            Console.Write("Quant.: ");
            try
            {
                products[posic].quant = Convert.ToSingle(Console.ReadLine());
            }
            catch { }

            if (products[posic].quant > 0 && products[posic].price > 0)
            {
                Console.Write("Expiration date (DD,MM,YYYY): ");

                try
                {
                    products[posic].expirationDate = Convert.ToDateTime(Console.ReadLine());
                }
                catch { }

                products[posic].iD = ident;
                ident++;

                using (StreamWriter sw = new StreamWriter("ID.txt", false))
                {
                    sw.Write(ident);
                }
                using (StreamWriter sw = new StreamWriter("productID.txt", true))
                {
                    sw.WriteLine(products[posic].iD);
                }
                using (StreamWriter sw = new StreamWriter("pruductName.txt", true))
                {
                    sw.WriteLine(products[posic].name);
                }
                using (StreamWriter sw = new StreamWriter("pruductPrice.txt", true))
                {
                    sw.WriteLine(products[posic].price);
                }
                using (StreamWriter sw = new StreamWriter("pruductQuant.txt", true))
                {
                    sw.WriteLine(products[posic].quant);
                }
                using (StreamWriter sw = new StreamWriter("productExpirationDate.txt", true))
                {
                    sw.WriteLine(products[posic].expirationDate.ToString("yyyy,MM,dd"));
                }

                posic++;

                add = 1;
                maxProd++;
                Main(args);
            }
            else
            {
                products[posic].price = 0;
            }
        }
        else
        {
            Console.Write("Stock Full!");
            Console.ReadKey();
        }
    }

    private static void ChangeQuantProd(ref Stock[] products)
    {
        int cod = maxProd + 1;
        string confirm;

        Console.Clear();
        Console.WriteLine("Change product quant.\n");
        Console.Write("Product cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod = BinSearch(products, cod, maxProd);

            if (cod >= 0)
            {
                if (products[cod].price > 0)
                {
                    Console.WriteLine($"\n( {products[cod].name} - {products[cod].quant} )\n");
                    Console.Write("confirm? (s/n): ");
                    confirm = Console.ReadLine();

                    if (confirm == "s" || confirm == "S")
                    {
                        Console.Write("New quant.: ");
                        try
                        {
                            products[cod].quant = Convert.ToSingle(Console.ReadLine());
                        }
                        catch { }

                        StreamWriter sw1 = new StreamWriter("pruductQuant.txt", false);
                        sw1.Close();

                        for (int i = 0; i < maxProd; i++)
                        {
                            if (products[i].price > 0)
                            {
                                using (StreamWriter sw = new StreamWriter("pruductQuant.txt", true))
                                {
                                    sw.WriteLine(products[i].quant);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.Write("Invalid product");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.Write("Invalid product");
                Console.ReadKey();
            }
        }
        catch { }
    }

    private static void ChangePrice(ref Stock[] products)
    {
        int cod = maxProd + 1;
        float price = 0;
        string confirm;

        Console.Clear();
        Console.WriteLine("Change product price\n");
        Console.Write("Product Cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod = BinSearch(products, cod, maxProd);

            if (cod >= 0)
            {
                if (products[cod].quant > 0)
                {
                    Console.WriteLine($"\n( {products[cod].name} - ${products[cod].price} )\n");
                    Console.Write("confirm? (s/n): ");
                    confirm = Console.ReadLine();

                    if (confirm == "s" || confirm == "S")
                    {
                        Console.Write("New price: ");
                        try
                        {
                            price = Convert.ToSingle(Console.ReadLine());
                        }
                        catch { }

                        if (price > 0)
                        {
                            products[cod].price = price;

                            StreamWriter sw1 = new StreamWriter("pruductPrice.txt", false);
                            sw1.Close();

                            for (int i = 0; i < maxProd; i++)
                            {
                                if (products[i].quant > 0)
                                {
                                    using (StreamWriter sw = new StreamWriter("pruductPrice.txt", true))
                                    {
                                        sw.WriteLine(products[i].price);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.Write("Invalid product");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.Write("Invalid product");
                Console.ReadKey();
            }
        }
        catch { }
    }

    private static void ChangeDate(ref Stock[] products)
    {
        int cod = maxProd + 1;
        string confirm;

        Console.Clear();
        Console.WriteLine("Change expiration date\n");
        Console.Write("Product Cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod = BinSearch(products, cod, maxProd);

            if (cod >= 0)
            {
                if (products[cod].price > 0)
                {
                    Console.WriteLine($"\n( {products[cod].name} - {products[cod].expirationDate.ToString("dd, MM, yyyy")} )\n");
                    Console.Write("confirm? (s/n): ");
                    confirm = Console.ReadLine();

                    if (confirm == "s" || confirm == "S")
                    {
                        Console.Write("New expiration date (DD,MM,YYYY): ");

                        try
                        {
                            products[cod].expirationDate = Convert.ToDateTime(Console.ReadLine());

                            StreamWriter sw1 = new StreamWriter("productExpirationDate.txt", false);
                            sw1.Close();

                            for (int i = 0; i < maxProd; i++)
                            {
                                if (products[i].price > 0)
                                {
                                    using (StreamWriter sw = new StreamWriter("productExpirationDate.txt", true))
                                    {
                                        sw.WriteLine(products[i].expirationDate.ToString("yyyy,MM,dd"));
                                    }
                                }
                            }
                        }
                        catch { }

                    }
                }
                else
                {
                    Console.Write("Invalid product");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.Write("Invalid product");
                Console.ReadKey();
            }
        }
        catch { }
    }

    private static void ChangeName(ref Stock[] products)
    {
        int cod = maxProd + 1;
        string confirm;

        Console.Clear();
        Console.WriteLine("Change name\n");
        Console.Write("Product Cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod = BinSearch(products, cod, maxProd);

            if (cod >= 0)
            {
                if (products[cod].price > 0)
                {
                    Console.WriteLine($"\n( {products[cod].name} - {products[cod].price} )\n");
                    Console.Write("confirm? (s/n): ");
                    confirm = Console.ReadLine();

                    if (confirm == "s" || confirm == "S")
                    {
                        Console.Write("New name: ");
                        try
                        {
                            products[cod].name = Console.ReadLine();
                        }
                        catch { }

                        StreamWriter sw1 = new StreamWriter("pruductName.txt", false);
                        sw1.Close();

                        for (int i = 0; i < maxProd; i++)
                        {
                            if (products[i].price > 0)
                            {
                                using (StreamWriter sw = new StreamWriter("pruductName.txt", true))
                                {
                                    sw.WriteLine(products[i].name);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.Write("Invalid product");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.Write("Invalid product");
                Console.ReadKey();
            }
        }
        catch { }
    }

    private static void DeleteProd(ref Stock[] products, string[] args)
    {
        int cod = maxProd + 1;
        string confirm;

        Console.Clear();
        Console.WriteLine("Delete product\n");
        Console.Write("Product Cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod = BinSearch(products, cod, maxProd);

            if (cod >= 0)
            { 
                if (products[cod].price > 0)
                {
                    Console.WriteLine($"\n( {products[cod].name} - {products[cod].price} )\n");
                    Console.Write("confirm? (s/n): ");
                    confirm = Console.ReadLine();
                    if (confirm == "s" || confirm == "S")
                    {
                        StreamWriter productiD1 = new StreamWriter("productID.txt", false);
                        productiD1.Close();
                        StreamWriter name1 = new StreamWriter("pruductName.txt", false);
                        name1.Close();
                        StreamWriter price1 = new StreamWriter("pruductPrice.txt", false);
                        price1.Close();
                        StreamWriter quant1 = new StreamWriter("pruductQuant.txt", false);
                        quant1.Close();
                        StreamWriter expirationDate1 = new StreamWriter("productExpirationDate.txt", false);
                        expirationDate1.Close();

                        for (int i = cod; i < maxProd; i++)
                        {
                            products[i].iD = products[i + 1].iD;
                            products[i].name = products[i + 1].name;
                            products[i].price = products[i + 1].price;
                            products[i].quant = products[i + 1].quant;
                            products[i].expirationDate = products[i + 1].expirationDate;
                        }

                        for (int i = 0; i < maxProd; i++)
                        {
                            if (products[i].price > 0)
                            {
                                using (StreamWriter sw = new StreamWriter("productID.txt", true))
                                {
                                    sw.WriteLine(products[i].iD);
                                }
                                using (StreamWriter sw = new StreamWriter("pruductName.txt", true))
                                {
                                    sw.WriteLine(products[i].name);
                                }
                                using (StreamWriter sw = new StreamWriter("pruductPrice.txt", true))
                                {
                                    sw.WriteLine(products[i].price);
                                }
                                using (StreamWriter sw = new StreamWriter("pruductQuant.txt", true))
                                {
                                    sw.WriteLine(products[i].quant);
                                }
                                using (StreamWriter sw = new StreamWriter("productExpirationDate.txt", true))
                                {
                                    sw.WriteLine(products[i].expirationDate.ToString("yyyy,MM,dd"));
                                }
                            }
                        }

                        if (posic > 0)
                        {
                            posic--;
                            delete = 1;
                            maxProd--;
                            Main(args);
                        }
                    }
                }
                else
                {
                    Console.Write("Invalid product");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.Write("Invalid product");
                Console.ReadKey();
            } 
        }
        catch { }
    }

    private static void ReturnProd(ref Stock[] products)
    {
        int cod = maxProd + 1;
        float quant = 0;
        string confirm;

        Console.Clear();
        Console.WriteLine("Return product\n");
        Console.Write("Product cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod = BinSearch(products, cod, maxProd);

            if (cod >= 0)
            {
                if (products[cod].price > 0)
                {
                    Console.WriteLine($"\n( {products[cod].name} - {products[cod].quant} )\n");
                    Console.Write("confirm? (s/n): ");
                    confirm = Console.ReadLine();

                    if (confirm == "s" || confirm == "S")
                    {
                        Console.Write("Returning quant.: ");
                        try
                        {
                            quant = Convert.ToSingle(Console.ReadLine());
                        }
                        catch { }

                        if (quant > 0)
                        {
                            products[cod].quant += quant;

                            StreamWriter sw1 = new StreamWriter("pruductQuant.txt", false);
                            sw1.Close();

                            for (int i = 0; i < maxProd; i++)
                            {
                                if (products[i].price > 0)
                                {
                                    using (StreamWriter sw = new StreamWriter("pruductQuant.txt", true))
                                    {
                                        sw.WriteLine(products[i].quant);
                                    }
                                }
                            }

                            using (StreamWriter sw = new StreamWriter("reportSales.txt", true))
                            {
                                sw.WriteLine($"(Cod.: {products[cod].iD}) {products[cod].name} - {quant} * ${products[cod].price} = ${products[cod].price * quant} (RETURN)");
                            }

                            totalReports -= products[cod].price * quant;

                            using (StreamWriter sw = new StreamWriter("totalReports.txt", false))
                            {
                                sw.Write(totalReports);
                            }
                        }
                        else
                        {
                            Console.Write("Must be higher than 0  ");
                            Console.ReadKey();
                        }
                    }
                }
                else
                {
                    Console.Write("Invalid product");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.Write("Invalid product");
                Console.ReadKey();
            }
        }
        catch { }
    }

    private static void SelectOptionStockMenu(ref string option)
    {
        if (edit == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            WriteAT("                                                                   Filter:", 0, 3);
            Console.ResetColor();
            Console.Write($"\n    Add product      Edit Prod.     Return prod.    Delete Prod.   {filterTxt}"); WriteAT("Main menu", 85, 4);
        }
        else if (edit == 1)
        {
            WriteAT("   Change quant.   Change Price     Change Date     Change name       Return", 0, 4);
        }
        else if (edit == 2)
        {
            WriteAT("   All products       Expired       Almost expi.      Return", 0, 4);
        }

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        WriteAT("+-------------------------------------------------------------------------------------------------+", 0, 0);
        Console.Write("\n|"); WriteAT("|", 98, 1);
        Console.Write("\n|-------------------------------------------------------------------------------------------------|\n|\n|\n|");
        WriteAT("|", 98, 3); WriteAT("|", 98, 4);  WriteAT("|", 98, 5);
        Console.ResetColor();

        //Check if the arrow keys are pressed (Observa se as setinhas foram apertadas)
        if (option == "RightArrow")
        {
            if (edit == 0)
            {
                if (xStock < 81)
                {
                    xStock += 16;
                }
                else if (xStock == 81)
                {
                    xStock = 1;
                }
            }
            else if (edit == 1)
            {
                if (xStock < 65)
                {
                    xStock += 16;
                }
                else if (xStock == 65)
                {
                    xStock = 1;
                }
            }
            else if (edit == 2)
            {
                if (xStock < 49)
                {
                    xStock += 16;
                }
                else if (xStock == 49)
                {
                    xStock = 1;
                }
            }
        }

        if (option == "LeftArrow")
        {
            if (xStock > 1 && edit == 1 || xStock > 1 && edit == 2 || xStock > 1 && edit == 0)
            {
                xStock -= 16;
            }
            else if (xStock == 1 && edit == 0)
            {
                xStock = 81;
            }
            else if (xStock == 1 && edit == 2)
            {
                xStock = 49;
            }
            else if (xStock == 1 && edit == 1)
            {
                xStock = 65;
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        WriteAT("+---------------+", xStock, 5);
        Console.ResetColor();

        Console.SetCursorPosition(0, 1);

        option = Convert.ToString(Console.ReadKey().Key);

        //Check if the arrow keys are pressed (Observa se as setinhas foram apertadas)
        if (option == "UpArrow" && limitUpStock == 1)
        {
            scrollStock ++;
        }
        if (option == "DownArrow" && limitDownStock == 1)
        {
            scrollStock --;
        }
    }

    private static void WriteProductsStockScreen(Stock[] products, ref int y, ref int yy)
    {
        WriteAT("   Cod.     Description                                     Price       Quant.       Expiration     ", 0, 7);
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        WriteAT("|-------------------------------------------------------------------------------------------------|", 0, 6);
        WriteAT("|", 0, 7); WriteAT("|", 9, 7); WriteAT("|", 57, 7); WriteAT("|", 69, 7); WriteAT("|", 82, 7); WriteAT("|", 98, 7);
        Console.WriteLine("\n|-------------------------------------------------------------------------------------------------|");
        Console.ResetColor();

        for (int i = 0; i < maxProd; i++)
        {
            if (products[i].price > 0)
            {
                //Writes the existing products based on the filter settings (Escreve os produtos existentes baseado nas configurações do filtro)
                if ((y + scrollStock) >= 9 && (y + scrollStock) < 28 && filter == 0)
                {
                    intervalChangeColor = products[i].expirationDate - today;
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    WriteAT("|        |                                               |           |            |               |", 0, y + scrollStock);
                    Console.ResetColor();
                    WriteAT($"{products[i].iD}", 4, y + scrollStock); WriteAT($"{products[i].name}", 12, y + scrollStock); WriteAT($"${products[i].price}", 60, y + scrollStock); WriteAT($"{products[i].quant}", 72, y + scrollStock);
                    if (intervalChangeColor.Days <= almostExpiring && intervalChangeColor.Days >= 0) { Console.ForegroundColor = ConsoleColor.Yellow; } else if (intervalChangeColor.Days < 0) { Console.ForegroundColor = ConsoleColor.Red; } 
                    WriteAT(products[i].expirationDate.ToString("dd, MM, yyyy"), 85, y + scrollStock); Console.ResetColor(); 
                }
                if (filter == 0)
                {
                    y++;
                }

                else if ((y + scrollStock) >= 9 && (y + scrollStock) < 28 && filter == 1)
                {
                    intervalChangeColor = products[i].expirationDate - today;

                    if (intervalChangeColor.Days < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        WriteAT("|        |                                               |           |            |               |", 0, y + scrollStock);
                        Console.ResetColor();
                        WriteAT($"{products[i].iD}", 4, y + scrollStock); WriteAT($"{products[i].name}", 12, y + scrollStock); WriteAT($"${products[i].price}", 60, y + scrollStock); WriteAT($"{products[i].quant}", 72, y + scrollStock);
                        Console.ForegroundColor = ConsoleColor.Red;
                        WriteAT(products[i].expirationDate.ToString("dd, MM, yyyy"), 85, y + scrollStock); Console.ResetColor();
                        y++;
                    }
                }
                else if ((y + scrollStock) >= 9 && (y + scrollStock) < 28 && filter == 2)
                {
                    intervalChangeColor = products[i].expirationDate - today;

                    if (intervalChangeColor.Days <= almostExpiring && intervalChangeColor.Days >= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        WriteAT("|        |                                               |           |            |               |", 0, y + scrollStock);
                        Console.ResetColor();
                        WriteAT($"{products[i].iD}", 4, y + scrollStock); WriteAT($"{products[i].name}", 12, y + scrollStock); WriteAT($"${products[i].price}", 60, y + scrollStock); WriteAT($"{products[i].quant}", 72, y + scrollStock);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        WriteAT(products[i].expirationDate.ToString("dd, MM, yyyy"), 85, y + scrollStock); Console.ResetColor();
                        y++;
                    }
                }
            }
        }

        if (y + scrollStock > 9 && y + scrollStock < 28)
        {
            yy = y + scrollStock;
            
        }
        else if (y + scrollStock >= 28)
        {
            yy = 28;
        }

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        WriteAT("+-------------------------------------------------------------------------------------------------+", 0, yy);

        if (scrollStock < 0)
        {
            WriteAT("|                                    ^^ SCROLL UP TO SEE MORE ^^                                  |", 0, 9);
            limitUpStock = 1;
        }
        else
        {
            limitUpStock = 0;
        }
        if ((y + scrollStock) >= 29)
        {
            WriteAT("|                                    vv SCROLL DOWN TO SEE MORE vv                                |", 0, 28);
            Console.Write("\n+-------------------------------------------------------------------------------------------------+");
            limitDownStock = 1;
        }
        else
        {
            limitDownStock = 0;
        }
        Console.ResetColor();
    }
    //End stock menu (Fim do Menu de estoque)





    //Binary search
    private static int BinSearch(Stock[] product, int key, int maxProd)
    {
        int middle, pos = -1, inf = 0, sup = maxProd - 1;

        while (inf <= sup)
        {
            middle = Convert.ToInt32(inf + ((sup - inf) / 2));
            if (product[middle].iD == key)
            {
                pos = middle;
                inf = sup + 1;
            }
            else if (product[middle].iD < key)
            {
                inf = middle + 1;
            }
            else
            {
                sup = middle - 1;
            } 
        }

        return pos;
    }
    //End binary search





    //Reports
    private static void ReportSales()
    {
        string option1 = "";
        int y = 1;

        while (option1 != "Enter")
        {
            y = 1;
            Console.Clear();
            Console.WriteLine("\n              Reports\n");
            Console.WriteLine(  "    Today     All time     Return \n\n");
            

            if (select == 0)
            {
                if (totalReports != 0)
                {
                    Console.WriteLine("----------------------------------------------\n");

                    using (StreamReader sr = new StreamReader("reportSales.txt"))
                    {
                        string reports;
                        while ((reports = sr.ReadLine()) != null)
                        {
                            if ((y + scrollReport) >= 1 && (y + scrollReport) < 24)
                            {
                                Console.WriteLine(reports);
                            }
                            y++;
                        }
                    }

                    using (StreamReader sr = new StreamReader("totalReports.txt"))
                    {
                        string total;
                        while ((total = sr.ReadLine()) != null)
                        {
                            totalReports = Convert.ToSingle(total);
                        }
                    }

                    Console.WriteLine("\n----------------------------------------------");

                    if ((y + scrollReport) < 23)
                    {
                        Console.Write($"Total: ${totalReports}");
                    }
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader("allReports.txt"))
                {
                    string reports1;
                    while ((reports1 = sr.ReadLine()) != null)
                    {
                        if ((y + scrollReport) >= 1 && (y + scrollReport) < 24)
                        {
                            Console.WriteLine(reports1);
                        }
                        y++;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            WriteAT("+-----------+", xReports, 4);

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (scrollReport < 0)
            {
                WriteAT("          ^^ SCROLL UP TO SEE MORE ^^                                             ", 0, 6);
                limitReportUp = 1;
            }
            else
            {
                limitReportUp = 0;
            }
            if ((y + scrollReport) >= 25)
            {
                WriteAT("          vv SCROLL DOWN TO SEE MORE vv                                                      ", 0, 29);
                limitReportDown = 1;
            }
            else
            {
                limitReportDown = 0;
            }
            Console.ResetColor();

            option1 = Convert.ToString(Console.ReadKey().Key);

            if (option1 == "UpArrow" && limitReportUp == 1)
            {
                scrollReport++;
            }
            if (option1 == "DownArrow" && limitReportDown == 1)
            {
                scrollReport--;
            }

            if (option1 == "RightArrow")
            {
                if (xReports < 24)
                {
                    xReports += 12;
                }
                else if (xReports == 24)
                {
                    xReports = 0;
                }
            }
            else if (option1 == "LeftArrow")
            {
                if (xReports > 0)
                {
                    xReports -= 12;
                }
                else if (xReports == 0)
                {
                    xReports = 24;
                }
            } 
        }

        if (xReports == 0)
        {
            select = 0;
            scrollReport = 0;
            ReportSales();
        }
        else if (xReports == 12)
        {
            select = 10;
            scrollReport = 0;
            ReportSales();
        }
        scrollReport = 0;
    }





    //End the program and saves the reports into a "all time reports"
    private static void EndProgram()
    {
        string confirm;

        Console.Clear();
        Console.Write("Confirm (s/n): ");
        confirm = Console.ReadLine();

        if (confirm == "s")
        {
            Console.Clear();
            Console.WriteLine(" Bye Bye!\n");

            if (totalReports != 0)
            {
                Console.WriteLine("----------------------------------------------\n");

                using (StreamReader sr = new StreamReader("reportSales.txt"))
                {
                    string reports;
                    while ((reports = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(reports);
                    }
                }

                using (StreamReader sr = new StreamReader("totalReports.txt"))
                {
                    string total;
                    while ((total = sr.ReadLine()) != null)
                    {
                        totalReports = Convert.ToSingle(total);
                    }
                }

                Console.WriteLine("\n----------------------------------------------");

                if (totalReports != 0)
                {
                    Console.Write($"Total: ${totalReports}    ");
                }

                using (StreamWriter sw = new StreamWriter("allReports.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd, MM, yyyy   HH:mm"));
                    sw.WriteLine("----------------------------------------------\n");

                    using (StreamReader sr = new StreamReader("reportSales.txt"))
                    {
                        string reports;
                        while ((reports = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(reports);
                        }
                    }

                    using (StreamReader sr = new StreamReader("totalReports.txt"))
                    {
                        string total;
                        while ((total = sr.ReadLine()) != null)
                        {
                            totalReports = Convert.ToSingle(total);
                        }
                    }

                    sw.WriteLine("\n----------------------------------------------");
                  
                    if (totalReports != 0)
                    {
                        sw.WriteLine($"Total: ${totalReports} \n");
                    }

                    sw.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++\n");
                }

                using (StreamWriter sw = new StreamWriter("reportSales.txt", false))
                {
                    sw.Write("");
                }

                using (StreamWriter sw = new StreamWriter("totalReports.txt", false))
                {
                    sw.Write("");
                }
            }

            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}