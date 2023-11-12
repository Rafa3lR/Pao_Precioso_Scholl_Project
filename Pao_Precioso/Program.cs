using Microsoft.VisualBasic.FileIO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;

internal class Program
{
    private static int posic = 0, xMenu, xStock, xSales, edit = 0, maxProd = 50, scrollStock = 0, scrollSales = 0, limitUpStock = 0, limitDownStock = 0, limitUpSales = 0, limitDownSales = 0;
    private static string reportSales = "", reportFinal = "";
    private static float totalSale = 0;
    private static int[] quantSale = new int[maxProd + 1];

    private static void Main(string[] args)
    {
        Stock[] products = new Stock[maxProd + 1];
        Menu(ref products);
    }

    //Data struct for the products
    struct Stock
    {
        public string name;
        public int quant;
        public float price;
        public DateTime expirationDate;
    }

    //Write something in a specified location in the console
    private static void WriteAT(string a, int x, int y)
    {
        try
        {
            Console.SetCursorPosition(x, y);
            Console.Write(a);
        }
        catch { }
    }



    //Writes a simple menu with options and a way to interact with the keyboard arrows
    private static void Menu(ref Stock[] products)
    {
        string option = "";

        while (option != "Enter")
        {
            Console.Clear();
            Console.WriteLine(" Welcome!");
            Console.WriteLine("(use arrow keys for navigation)\n");
            Console.WriteLine("   Sales     Stock");

            if (option == "LeftArrow" || option == "RightArrow")
            {
                if (xMenu == 0)
                {
                    xMenu += 10;
                }
                else if (xMenu == 10)
                {
                    xMenu = 0;
                }
            }

            //WriteAT("|", xMenu, 3); WriteAT("|", xMenu + 10, 3);
            //WriteAT("+---------+", xMenu, 2);
            WriteAT("+---------+", xMenu, 4);

            option = Convert.ToString(Console.ReadKey().Key);
        }

        if (xMenu == 0)
        {
            SalesMenu(ref products);
        }
        else if(xMenu == 10)
        {
            StockMenu(ref products);
        }
        else if(xMenu == 20)
        {
            //ReportSales();
        }

        Menu(ref products);
    }



    //Sales Menu
    private static void SalesMenu(ref Stock[] products)
    {
        string option = "";
        int y = 8; 
        while (option != "Enter")
        {
            y = 8;
            Console.Clear();
            Console.WriteLine(" Sales");

            WriteProductsSalesScreen(products, ref y);

            SelectOptionSalesMenu(ref option);
        }

        if (xSales == 0)
        {
            AddProductSales(ref products);
        }
        else if (xSales == 16)
        {
            ConsultStock(products, ref y);
        }
        else if (xSales == 32)
        {
            DeleteProdSales(ref products);
        }
        else if (xSales == 48)
        {
            EndSale(products);
        }
        else if (xSales == 64)
        {
            CancelSale(ref products);
        }

        SalesMenu(ref products);
    }

    private static void AddProductSales(ref Stock[] products)
    {
        int opcao = maxProd + 1, quant = 0;
        string confirm = "";

        Console.Clear();
        Console.WriteLine("Add. product sale\n");
        Console.Write("Product cod.: ");
        try
        {
            opcao = Convert.ToInt32(Console.ReadLine());
            opcao -= 1;
        }
        catch { }

        if (products[opcao].quant > 0)
        {
            Console.Write($"|  {products[opcao].name} - {products[opcao].price}  |  Confirm? (s/n): ");
            confirm = Console.ReadLine();

            if (confirm == "s")
            {
                Console.Write("Quant.: ");
                try
                {
                    quant = Convert.ToInt32(Console.ReadLine());
                }
                catch { }
                
                if (products[opcao].quant > quant)
                {
                    products[opcao].quant -= quant;
                    quantSale[opcao] += quant;
                    totalSale += quant * products[opcao].price;
                    
                }
                else
                {
                    Console.Write("Not enough balance! ");
                    Console.ReadKey();
                    SalesMenu(ref products);
                }
            }
            else
            {
                SalesMenu(ref products);
            }
        }
        else
        {
            Console.Write("Invalid product! ");
            Console.ReadKey();
            SalesMenu(ref products);
        }

        SalesMenu(ref products);
    }

    private static void DeleteProdSales(ref Stock[] products)
    {
        int opcao = maxProd + 1;
        string confirm = "";

        Console.Clear();
        Console.WriteLine("Delete product sale\n");
        Console.Write("Product cod.: ");
        try
        {
            opcao = Convert.ToInt32(Console.ReadLine());
            opcao -= 1;
        }
        catch { }

        if (quantSale[opcao] > 0)
        {
            Console.Write($"|  {products[opcao]}  |  (Confirm? [s/n])   ");
            confirm = Console.ReadLine();

            if (confirm == "s")
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
            SalesMenu(ref products);
        }

        SalesMenu(ref products);
    }

    private static void EndSale(Stock[] products)
    {
        string confirm = "";

        Console.Clear();
        Console.Write("Confirm? (s/n)  ");
        confirm = Console.ReadLine();
        if (confirm == "s")
        {
            for (int i = 0; i < maxProd; i++)
            {
                if (quantSale[i] > 0)
                {
                    reportSales += $"| (Cod.:{i + 1}) {products[i].name} * {quantSale[i]} = {products[i].price * quantSale[i]}\n";
                    quantSale[i] = 0;
                }
            }

            Menu(ref products);
        }
        else
        {
            SalesMenu(ref products);
        }
    }

    private static void CancelSale(ref Stock[] products)
    {
        string confirm = "";

        Console.Clear();
        Console.Write("Confirm? (s/n)  ");
        confirm = Console.ReadLine();
        if (confirm == "s")
        {
            for(int i = 0; i < maxProd; i++)
            {
                products[i].quant += quantSale[i];
                quantSale[i] = 0;
            }

            Menu(ref products);
        }
        else
        {
            SalesMenu(ref products);
        }
    }

    private static void WriteProductsSalesScreen(Stock[] products, ref int y)
    {
        WriteAT($"Total: {totalSale}", 0, 4);
        WriteAT("-----------------------------------------------", 0, 5);
        Console.WriteLine("\n|  Cod.  |  Description  |  Quant.  |  Price  |");
        Console.WriteLine("-----------------------------------------------");

        for (int i = 0; i < maxProd; i++)
        {
            if (quantSale[i] > 0)
            {
                if ((y + scrollSales) >= 7 && (y + scrollSales) < 27)
                {
                    WriteAT($"{i + 1}", 4, y + scrollSales); WriteAT($"{products[i].name}", 12, y + scrollSales); WriteAT($"{quantSale[i]}", 28, y + scrollSales); WriteAT($"${products[i].price * quantSale[i]}", 39, y + scrollSales);
                    WriteAT("|", 0, y + scrollSales); WriteAT(" |", 8, y + scrollSales); WriteAT(" |", 24, y + scrollSales); WriteAT(" |", 35, y + scrollSales); WriteAT(" |", 45, y + scrollSales);
                    WriteAT("-----------------------------------------------", 0, (y + scrollSales) + 1);
                }
                y += 2;
            }
        }

        if (scrollSales < 0)
        {
            WriteAT("----------------------------------------------------------------", 0, 7);
            WriteAT("|                ^^  SCROLL UP TO SEE MORE ^^                  |", 0, 8);
            WriteAT("----------------------------------------------------------------", 0, 9);
            limitUpSales = 1;
        }
        else
        {
            limitUpSales = 0;
        }
        if ((y + scrollSales) >= 29)
        {
            WriteAT("----------------------------------------------------------------", 0, 27);
            WriteAT("|                vv  SCROLL DOWN TO SEE MORE vv                |  ", 0, 28);
            WriteAT("----------------------------------------------------------------", 0, 29);
            limitDownSales = 1;
        }
        else
        {
            limitDownSales = 0;
        }
    }

    private static void ConsultStock(Stock[] products, ref int y)
    {
        //Console.WriteLine("|0        |9               |25         |35          |46              |61"); X position of each "|"
        string option = "";
        int scrollConsultStock = 0, limitUpConsultStock = 0, limitDownConsultStock = 0;

        while (option != "Enter")
        {
            Console.Clear();
            Console.WriteLine(" Stock");
            y = 5;
            WriteAT("________________________________________________________________", 0, 2);
            Console.WriteLine("\n|  Cod.  |  Description  |  Price  |  Quant.  |  Expiration    |");
            Console.WriteLine("----------------------------------------------------------------");

            for (int i = 0; i < maxProd; i++)
            {
                if (products[i].quant > 0)
                {
                    if ((y + scrollConsultStock) >= 5 && (y + scrollConsultStock) < 25)
                    {
                        WriteAT($"{i + 1}", 4, y + scrollConsultStock); WriteAT($"{products[i].name}", 12, y + scrollConsultStock); WriteAT($"${products[i].price}", 28, y + scrollConsultStock); WriteAT($"{products[i].quant}", 38, y + scrollConsultStock); WriteAT(products[i].expirationDate.ToString("dd, MM, yyyy"), 49, y + scrollConsultStock);
                        WriteAT("|", 0, y + scrollConsultStock); WriteAT(" |", 8, y + scrollConsultStock); WriteAT(" |", 24, y + scrollConsultStock); WriteAT(" |", 34, y + scrollConsultStock); WriteAT(" |", 45, y + scrollConsultStock); WriteAT(" |", 62, y + scrollConsultStock);
                        WriteAT("----------------------------------------------------------------", 0, (y + scrollConsultStock) + 1);
                    }
                    y += 2;
                }
            }

            if (scrollConsultStock < 0)
            {
                WriteAT("----------------------------------------------------------------", 0, 4);
                WriteAT("|                ^^  SCROLL UP TO SEE MORE ^^                  |", 0, 5);
                WriteAT("----------------------------------------------------------------", 0, 6);
                limitUpConsultStock = 1;
            }
            else
            {
                limitUpConsultStock = 0;
            }
            if ((y + scrollConsultStock) >= 27)
            {
                WriteAT("----------------------------------------------------------------", 0, 24);
                WriteAT("|                vv  SCROLL DOWN TO SEE MORE vv                |  ", 0, 25);
                WriteAT("----------------------------------------------------------------", 0, 26);
                limitDownConsultStock = 1;
            }
            else
            {
                limitDownConsultStock = 0;
            }

            option = Convert.ToString(Console.ReadKey().Key);

            if (option == "UpArrow" && limitUpConsultStock == 1)
            {
                scrollConsultStock += 2;
            }
            if (option == "DownArrow" && limitDownConsultStock == 1)
            {
                scrollConsultStock -= 2;
            }
        }
    }

    private static void SelectOptionSalesMenu(ref string option)
    {
        WriteAT("   Add product    Consult Stock    Delete Prod.      End sale         Cancel  ", 0, 2);

        if (option == "RightArrow")
        {
            if (xSales < 64)
            {
                xSales += 16;
            }
            else if (xSales == 64)
            {
                xSales = 0;
            }
        }

        if (option == "LeftArrow")
        {
            if (xSales > 0)
            {
                xSales -= 16;
            }
            else if (xSales == 0)
            {
                xSales = 64;
            }
        }

        //WriteAT("|", xStock, 2); WriteAT("|", xStock + 16, 2);
        //WriteAT("+---------------+", xStock, 1);
        WriteAT("+---------------+", xSales, 3);

        option = Convert.ToString(Console.ReadKey().Key);

        if (option == "UpArrow" && limitUpSales == 1)
        {
            scrollSales += 2;
        }
        if (option == "DownArrow" && limitDownSales == 1)
        {
            scrollSales -= 2;
        }
    }
    //End sales menu





    //Stock Menu
    private static void StockMenu(ref Stock[] products)
    {
        string option = "";

        while (option != "Enter")
        {
            int y = 7;
            Console.Clear();
            Console.WriteLine(" Stock          Today: " + DateTime.Now.ToString("dd, MM, yyyy"));

            WriteProductsStockScreen(products, ref y);

            SelectOptionStockMenu(ref option);
        }

        if (xStock == 0 && edit == 0)
        {
            AddProductStock(ref products);
        }
        else if (xStock == 0 && edit == 1)
        {
            ChangeQuantProd(ref products);
        }
        else if (xStock == 16 && edit == 0)
        {
            edit = 1;
            StockMenu(ref products);
        }
        else if (xStock == 16 && edit == 1)
        {
            ChangePrice(ref products);
        }
        else if (xStock == 32 && edit == 0)
        {
            //ReturnProd();
        }
        else if (xStock == 32 && edit == 1)
        {
            ChangeDate(ref products);
        }
        else if (xStock == 48 && edit == 1)
        {
            edit = 0;
            xStock = 16;
        }
        else if (xStock == 48 && edit == 0)
        {
            DeleteProd(ref products);
        }
        else if (xStock == 64 && edit == 0)
        {
            Menu(ref products);
        }

        StockMenu(ref products);
    }

    private static void AddProductStock(ref Stock[] products)
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
                products[posic].quant = Convert.ToInt32(Console.ReadLine());
            }
            catch { }
            Console.Write("Expiration date (YYYY, MM, DD): ");
            try
            {
                products[posic].expirationDate = Convert.ToDateTime(Console.ReadLine());
            }
            catch { }
            if (products[posic].quant > 0 && products[posic].price > 0)
            {
                posic++;
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
        Console.Clear();
        Console.WriteLine("Change product quant.\n");
        Console.Write("Product cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod -= 1;
        }
        catch { }
        if(cod >= 0 && cod < maxProd)
        {
            if (products[cod].quant > 0)
            {
                Console.Write("Quant.: ");
                products[cod].quant = Convert.ToInt32(Console.ReadLine());
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

    private static void ChangePrice(ref Stock[] products)
    {
        int cod = maxProd + 1;
        Console.Clear();
        Console.WriteLine("Change product price\n");
        Console.Write("Product Cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod -= 1;
        }
        catch { }

        if (cod >= 0 && cod < maxProd)
        {
            if (products[cod].quant > 0)
            {
                Console.Write("Price: ");
                products[cod].price = Convert.ToInt32(Console.ReadLine());
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

    private static void ChangeDate(ref Stock[] products)
    {
        int cod = maxProd + 1;
        Console.Clear();
        Console.WriteLine("Chenge expiration date\n");
        Console.Write("Product Cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod -= 1;
        }
        catch { }

        if (cod >= 0 && cod < maxProd)
        {
            if (products[cod].quant > 0)
            {
                Console.Write("Expiration date (YYYY,MM,DD): ");
                products[cod].expirationDate = Convert.ToDateTime(Console.ReadLine());
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

    private static void DeleteProd(ref Stock[] products)
    {
        int cod = maxProd + 1;
        Console.Clear();
        Console.WriteLine("Delete product\n");
        Console.Write("Product Cod.: ");
        try
        {
            cod = Convert.ToInt32(Console.ReadLine());
            cod -= 1;
        }
        catch { }

        if (cod >= 0 && cod < maxProd)
        {
            if (products[cod].quant > 0)
            {
                for (int i = cod; i < maxProd; i++)
                {
                    products[i].name = products[i +1 ].name;
                    products[i].price = products[i + 1].price;
                    products[i].quant = products[i + 1].quant;
                    products[i].expirationDate = products[i + 1].expirationDate;
                }
                if (posic > 0)
                {
                    posic--;
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

    private static void SelectOptionStockMenu(ref string option)
    {
        if (edit == 0)
        {
            WriteAT("   Add product      Edit Prod.     Return prod.    Delete Prod.     Main menu  ", 0, 2);
        }
        if (edit == 1)
        {
            WriteAT("  Change quant.    Change Price    Change Date       Return" , 0, 2);
        }

        if (option == "RightArrow")
        {
            if (xStock < 64 && edit == 0)
            {
                xStock += 16;
            }
            else if (xStock == 64 && edit == 0)
            {
                xStock = 0;
            }

            if (xStock < 48 && edit == 1)
            {
                xStock += 16;
            }
            else if (xStock == 48 && edit == 1)
            {
                xStock = 0;
            }
        }

        if (option == "LeftArrow")
        {
            if (xStock > 0 && edit == 0)
            {
                xStock -= 16;
            }
            else if (xStock == 0 && edit == 0)
            {
                xStock = 64;
            }

            if (xStock > 0 && edit == 1)
            {
                xStock -= 16;
            }
            else if (xStock == 0 && edit == 1)
            {
                xStock = 48;
            }
        }

        //WriteAT("|", xStock, 2); WriteAT("|", xStock + 16, 2);
        //WriteAT("+---------------+", xStock, 1);
        WriteAT("+---------------+", xStock, 3);

        option = Convert.ToString(Console.ReadKey().Key);

        if (option == "UpArrow" && limitUpStock == 1)
        {
            scrollStock += 2;
        }
        if (option == "DownArrow" && limitDownStock == 1)
        {
            scrollStock -= 2;
        }
    }

    private static void WriteProductsStockScreen(Stock[] products, ref int y)
    {
        //Console.WriteLine("|0        |9               |25         |35          |46              |61"); X position of each "|"
        WriteAT("________________________________________________________________", 0, 4);
        Console.WriteLine("\n|  Cod.  |  Description  |  Price  |  Quant.  |  Expiration    |");
        Console.WriteLine("----------------------------------------------------------------");

        for (int i = 0; i < maxProd; i++)
        {
            if (products[i].quant > 0)
            {
                if ((y + scrollStock) >= 7 && (y + scrollStock) < 27)
                {
                    WriteAT($"{i + 1}", 4, y + scrollStock); WriteAT($"{products[i].name}", 12, y + scrollStock); WriteAT($"${products[i].price}", 28, y + scrollStock); WriteAT($"{products[i].quant}", 38, y + scrollStock); WriteAT(products[i].expirationDate.ToString("dd, MM, yyyy"), 49, y + scrollStock);
                    WriteAT("|", 0, y + scrollStock); WriteAT(" |", 8, y + scrollStock); WriteAT(" |", 24, y + scrollStock); WriteAT(" |", 34, y + scrollStock); WriteAT(" |", 45, y + scrollStock); WriteAT(" |", 62, y + scrollStock);
                    WriteAT("----------------------------------------------------------------", 0, (y + scrollStock) + 1);
                }
                y += 2;
            }
        }

        if (scrollStock < 0)
        {
            WriteAT("----------------------------------------------------------------", 0, 6);
            WriteAT("|                ^^  SCROLL UP TO SEE MORE ^^                  |", 0, 7);
            WriteAT("----------------------------------------------------------------", 0, 8);
            limitUpStock = 1;
        }
        else
        {
            limitUpStock = 0;
        }
        if ((y + scrollStock) >= 29)
        {
            WriteAT("----------------------------------------------------------------", 0, 26);
            WriteAT("|                vv  SCROLL DOWN TO SEE MORE vv                |  ", 0, 27);
            WriteAT("----------------------------------------------------------------", 0, 28);
            limitDownStock = 1;
        }
        else
        {
            limitDownStock = 0;
        }
    }
    //End stock menu
}