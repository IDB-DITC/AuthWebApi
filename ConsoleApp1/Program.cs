namespace ConsoleApp1
{
  internal class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello, World!");
      Circle c1 = new Circle();
      Circle c3 = new Circle(5);
      Circle c2 = new Circle() { Radius =33};



    }
  }


  public class Circle(int radius = 1)
  {
    

    public int Radius { get => radius; set => radius = value; }


    //public Circle(int r=1)
    //{

    //  Radius = r;
    //}
    //public Circle()
    //{
    //  Radius = 1;
    //}
  }
}
