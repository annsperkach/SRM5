using System;

namespace Lab5SRM
{
    class Program
 {
    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Black; //налаштування кольору консолі
        Console.BackgroundColor = ConsoleColor.Yellow;

        int n = 5;
        double[] x = { -3, -1, 1, 3, 5 };                   //задані вузли інтерполяції                        
        double[] y = { -2.439745, -1.479426, 1.479426, 2.439745, 2.308448 };        //інтерполянти функції
        Console.WriteLine("\nInterpolation nodes and interpolants::");
            for (int i = 0; i < n; i++)
        {
                    //Вузли інтерполяцій та інтерполянти
                Console.WriteLine("x[{0}] = {1}    y[{2}] = {3:f6}", i, x[i], i, y[i]);  //форматований вивід x та y
        }
        Console.WriteLine("_________________________________________________________________________________________________________________________");
        Console.WriteLine("\nNewton's interpolation polynomial:");
        NewtonMethod(x, y, n);
        Console.WriteLine("_________________________________________________________________________________________________________________________");
        Console.WriteLine("\nInterpolation by cubic splines: ");
        for (int i = 0; i < n; i++)
        SplineInterpolation(x, y, n, x[i]);
    }

        static void NewtonMethod(double[] x, double[] y, int n)   //Інтерполяційний поліном Ньютона
        {
            double[] del1 = new double[n - 1];     //створюємо змінні, за якими будемо рахувати роздільну різницю першого та другого роду
            double[] del2 = new double[n - 2];
            double[] del3 = new double[n - 3];
            double[] del4 = new double[n - 4];

            ////////роздільна різниця першого роду///////
            for (int m = 0; m < n - 1; m++)
            {
                del1[m] = (y[m] - y[m + 1]) / (x[m] - x[m + 1]);
            }

            ////////роздільна різниця другого роду///////
            for (int j = 0; j < n - 2; j++)
            {
                del2[j] = (del1[j] - del1[j + 1]) / (x[j] - x[j + 2]);
            }
            for (int i = 0; i < n - 3; i++)
            {
                del3[i] = (del2[i] - del2[i + 1]) / (x[i] - x[i + 3]);
            }
            for (int i = 0; i < n - 4; i++)
            {
                del4[i] = (del3[i] - del3[i + 1]) / (x[i] - x[i + 4]);
            }
            ///Поліном Ньютона у формі Горнера:///
            Console.WriteLine($"{y[0]:f6}+{del1[0]:f5}*(x-({x[0]}))+{del2[0]:f5}*(x-({x[0]}))*(x-({x[1]}))+({del3[0]:f5})*(x-({x[0]}))*(x-({x[1]}))*(x-{x[2]})+{del4[0]:f5}*(x-({x[0]}))*(x-({x[1]}))*(x-{x[2]})*(x-{x[3]})");

            //Розрахуємо значення похибки//
            Console.WriteLine("\nLet's calculate the error by the formula |Pn(x)-y(x)|:");
            double[] e = new double[n - 1];
            e[0] = (del1[0] - y[0]);
            e[1] = (del2[0] - y[1]);
            e[2] = (del3[0] - y[2]);
            e[3] = (del4[0] - y[3]);
            Console.WriteLine($"e1 = {e[0]-2.919:f6}");
            Console.WriteLine($"e2 = {e[1] - 1.729:f6}");
            Console.WriteLine($"e3 = {e[2] + 1.5:f6}");
            Console.WriteLine($"e4 = {e[3] + 2.42:f6}");
        }
     
    static double SplineInterpolation(double[] X, double[] Y, int n, double x0)   //Інтерполяція кубічними сплайнами
    {
        double[] a = new double[n - 1];  //Fi(xі) = ai
        double[] b = new double[n - 1];  //F’i(xі) = bi
        double[] d = new double[n - 1];  //знаходиться за формулою с(і)-с(і-1)/h(i)
        double[] h = new double[n - 1];  //знаходиться за формулою х[i + 1] - х[i]; 

        double[,] F = new double[n - 1, n];   //кубічний поліном
        double[] spline = new double[n];

        for (int i = 0; i < n - 1; i++)
        {
            a[i] = Y[i];
            h[i] = X[i + 1] - X[i]; 
        }

        F[0, 0] = 1;
        F[n - 2, n - 2] = 1;
            ///формула для обчислення коефіцієнтів сплайну///
            for (int i = 1; i < n - 2; i++)
        {
            F[i, i - 1] = h[i - 1];    //ліва частина формули
            F[i, i] = 2 * (h[i - 1] + h[i]);
            F[i, i + 1] = h[i];

           spline[i] = 3 * (((Y[i + 1] - Y[i]) / h[i]) - ((Y[i] - Y[i - 1]) / h[i - 1]));    //права частина формули
            }
        double[] c = ProgonMethod(spline, n - 1, F);   // обчислення коефіцієнтів c можна провести за допомогою методу прогону для трьохдіагональної матриці

        for (int i = 0; i < n - 1; i++)
        {
            if (i != n - 2)
            {
                d[i] = (c[i + 1] - c[i]) / (3 * h[i]);
                b[i] = ((Y[i + 1] - Y[i]) / h[i]) - h[i] * (c[i + 1] + 2 * c[i]) / 3;
            }
            else
            {
                d[i] = (-1) * (c[i] / (3 * h[i]));
                b[i] = ((Y[i] - Y[i - 1]) / h[i]) - ((2 * h[i] * c[i]) / 3);

            }
        }
        d[n - 2] = -c[n - 2] / (3 * h[n - 2]);
        b[n - 2] = ((Y[n - 1] - Y[n - 2]) / h[n - 2]) - 2 * h[n - 2] * c[n - 2] / 3;
        int k = 0;
        for (int i = 1; i < n; i++)
        {
            if (x0 >= X[i - 1] && x0 <= X[i])
            {
                k = i - 1;
            }
        }

            ////після знайдення всіх значень, за формулою порахуємо кубічний поліном////
        double x = x0 - X[k];
        double y = a[k] + b[k] * x + c[k] * Math.Pow(x, 2) + d[k] * Math.Pow(x, 3);
        Console.WriteLine("_________________________________________________________________________________________________________________________");
        Console.WriteLine($"Splice coefficients for {y:f6}");
        Console.WriteLine($"a = {a[k]:f6}");
        Console.WriteLine($"b = {b[k]:f6}");
        Console.WriteLine($"c = {c[k]:f6}");
        Console.WriteLine($"d = {d[k]:f6}\n");

        return y;
    }

    static double[] ProgonMethod(double[] b, int n, double[,] F)
    {
        double[] Сoefficients = new double[n];
        int n1 = n - 1;

        double e = F[0, 0];
        double[] a1 = new double[n];
        double[] b1 = new double[n];
        a1[0] = -F[0, 1] / e;   
        b1[0] = b[0] / e;       

        for (int i = 1; i < n1; i++)
        {
            e = F[i, i] + F[i, i - 1] * a1[i - 1];                //рахуємо знаменник e
            a1[i] = -F[i, i + 1] / e;                             //Ai=- ci/ei
                b1[i] = (b[i] - F[i, i - 1] * b1[i - 1]) / e;     //Bi=(di-ai*Bi-1)/ei
        }
            Сoefficients[n1] = b1[n1];
        for (int i = n1 - 1; i >= 0; i--)
        {
                Сoefficients[i] = a1[i] * Сoefficients[i + 1] + b1[i]; //за цією формулою знаходимо значення прогоночних коефіцієнтів 
            }
        return Сoefficients;
    }
 }
}

