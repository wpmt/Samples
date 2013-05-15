using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Reflection;

namespace testlocal.Class
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Person obj = new Person();
            obj.FirstName = "sujeet";
            CreateControlsUsingObjects(obj);
        }



        private void CreateControlsUsingObjects(Person obj)
        {
            List<Person> objList=new List<Person>();

            objList.Add(obj);
            Grid rootGrid = new Grid();
            rootGrid.Margin = new Thickness(10.0);

            rootGrid.ColumnDefinitions.Add(
               new ColumnDefinition() { Width = new GridLength(100.0) });
            rootGrid.ColumnDefinitions.Add(
                 new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            rootGrid.ColumnDefinitions.Add(
                new ColumnDefinition() { Width = new GridLength(100.0) });
            rootGrid.ColumnDefinitions.Add(
                   new ColumnDefinition() { Width = new GridLength(100.0) });
   

                PropertyInfo[] propertyInfos;
                propertyInfos = typeof(Person).GetProperties();
                rootGrid.RowDefinitions.Add(CreateRowDefinition());
                int j = 1;
               
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (propertyInfo.PropertyType.Name == "String")
                    {

                        rootGrid.RowDefinitions.Add(CreateRowDefinition());

                        var Label = CreateTextBlock(propertyInfo.Name, j, 0);
                        rootGrid.Children.Add(Label);

                        var Textbox = CreateTextBox(j, 1);
                        rootGrid.Children.Add(Textbox);
                        j++; 
                    }
                    if (propertyInfo.PropertyType.Name == "Boolean")
                    {
                        rootGrid.RowDefinitions.Add(CreateRowDefinition());

                        var Label = CreateTextBlock(propertyInfo.Name, j, 0);
                        rootGrid.Children.Add(Label);

                        var Textbox =  CreateCheckBox(j, 1);
                        rootGrid.Children.Add(Textbox);
                        j++;
                    }

                   
                    
                }
                rootGrid.RowDefinitions.Add(CreateRowDefinition());
                var Button = CreateButton("Save",j + 1, 1);
                Button.Click += new RoutedEventHandler(button_Click);

                rootGrid.Children.Add(Button);
                LayoutRoot.Children.Add(rootGrid);
            

            
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Saved Successfully");
        }

        private Button CreateButton(string text, int row, int column )
        {
            Button tb = new Button() { Content = text, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5, 8, 0, 5) };
            tb.Width = 90;
            tb.Height = 25;
            tb.Margin = new Thickness(5);
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);            
            return tb;
        }



        private TextBlock CreateTextBlock(string text, int row, int column)
        {
            string[] aa = BreakUpperCB(text);
            string prop = "";
            for (int i = 0; i < aa.Length; i++)
            {
                 prop = prop +" "+ aa[i];
            }
            TextBlock tb = new TextBlock() { Text = prop, Margin = new Thickness(5, 8, 0, 5) };
            tb.MinWidth = 90;
            //tb.FontWeight
            tb.FontWeight = FontWeights.Bold;
            //tb.FontStyle=new System.Windows.FontStyle(){ FontWeight="Bold" ,  
            tb.Margin = new Thickness(5);
            var bc = new BrushConverter(); 
            tb.Foreground = (Brush)bc.ConvertFrom("#FF2D72BC"); 
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);           
            return tb;
        }

        private TextBox CreateTextBox( int row, int column)
        {
            TextBox tb = new TextBox();
            tb.Margin = new Thickness(5);
            tb.Height = 22;
            tb.Width = 150;

            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);

            return tb;
        }


        private CheckBox CreateCheckBox(int row, int column)
        {
            CheckBox cb = new CheckBox();
            cb.Margin = new Thickness(5);
            cb.Height = 22;
            cb.MinWidth = 50;

            Grid.SetColumn(cb, column);
            Grid.SetRow(cb, row);

            return cb;
        }

        private RowDefinition CreateRowDefinition()
        {
            RowDefinition RowDefinition = new RowDefinition();
            RowDefinition.Height = GridLength.Auto;
            return RowDefinition;
        }


        public string[] BreakUpperCB(string sInput)
        {
            StringBuilder[] sReturn = new StringBuilder[1];
            sReturn[0] = new StringBuilder(sInput.Length);
            const string CUPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int iArrayCount = 0;
            for (int iIndex = 0; iIndex < sInput.Length; iIndex++)
            {
                string sChar = sInput.Substring(iIndex, 1); // get a char
                if ((CUPPER.Contains(sChar)) && (iIndex > 0))
                {
                    iArrayCount++;
                    System.Text.StringBuilder[] sTemp = new System.Text.StringBuilder[iArrayCount + 1];
                    Array.Copy(sReturn, 0, sTemp, 0, iArrayCount);
                    sTemp[iArrayCount] = new StringBuilder(sInput.Length);
                    sReturn = sTemp;
                }
                sReturn[iArrayCount].Append(sChar);
            }
            string[] sReturnString = new string[iArrayCount + 1];
            for (int iIndex = 0; iIndex < sReturn.Length; iIndex++)
            {
                sReturnString[iIndex] = sReturn[iIndex].ToString();
            }
            return sReturnString;
        }







































        //private void CreateControlsUsingObjects()
        //{
        //    // <Grid Margin="10"> 
        //    Grid rootGrid = new Grid();
        //    rootGrid.Margin = new Thickness(10.0);

        //    // <Grid.ColumnDefinitions> 
        //    //   <ColumnDefinition Width="100" /> 
        //    //   <ColumnDefinition Width="*" /> 
        //    //</Grid.ColumnDefinitions> 

        //    rootGrid.ColumnDefinitions.Add(
        //        new ColumnDefinition() { Width = new GridLength(100.0) });
        //    rootGrid.ColumnDefinitions.Add(
        //        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

        //    //<Grid.RowDefinitions> 
        //    //  <RowDefinition Height="Auto" /> 
        //    //  <RowDefinition Height="Auto" /> 
        //    //  <RowDefinition Height="Auto" /> 
        //    //  <RowDefinition Height="*" /> 
        //    //</Grid.RowDefinitions> 

        //    rootGrid.RowDefinitions.Add(
        //        new RowDefinition() { Height = GridLength.Auto });
        //    rootGrid.RowDefinitions.Add(
        //        new RowDefinition() { Height = GridLength.Auto });
        //    rootGrid.RowDefinitions.Add(
        //        new RowDefinition() { Height = GridLength.Auto });
        //    rootGrid.RowDefinitions.Add(
        //        new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

        //    //<TextBlock Text="First Name" 
        //    //           Height="19" 
        //    //           Margin="0,7,31,4" /> 

        //    var firstNameLabel = CreateTextBlock("First Name", 19, new Thickness(0, 7, 31, 4), 0, 0);
        //    rootGrid.Children.Add(firstNameLabel);

        //    //<TextBox x:Name="FirstName" 
        //    //         Margin="3" 
        //    //         Grid.Row="0" 
        //    //         Grid.Column="1" /> 

        //    var firstNameField = CreateTextBox(new Thickness(3), 0, 1);
        //    rootGrid.Children.Add(firstNameField);

        //    //<TextBlock Text="Last Name" 
        //    //           Margin="0,7,6,3" 
        //    //           Grid.Row="1" 
        //    //           Height="20" /> 

        //    var lastNameLabel = CreateTextBlock("Last Name", 20, new Thickness(0, 7, 6, 3), 1, 0);
        //    rootGrid.Children.Add(lastNameLabel);


        //    //<TextBox x:Name="LastName" 
        //    //         Margin="3" 
        //    //         Grid.Row="1" 
        //    //         Grid.Column="1" /> 

        //    var lastNameField = CreateTextBox(new Thickness(3), 1, 1);
        //    rootGrid.Children.Add(lastNameField);


        //    //<TextBlock Text="Date of Birth" 
        //    //           Grid.Row="2" 
        //    //           Margin="0,9,0,0" 
        //    //           Height="21" /> 

        //    var dobLabel = CreateTextBlock("Date of Birth", 21, new Thickness(0, 9, 0, 0), 2, 0);
        //    rootGrid.Children.Add(dobLabel);

        //    //<DatePicker x:Name="DateOfBirth" 
        //    //            Margin="3" 
        //    //            Grid.Row="2" 
        //    //            Grid.Column="1" /> 

        //    DatePicker picker = new DatePicker();
        //    picker.Margin = new Thickness(3);
        //    Grid.SetRow(picker, 2);
        //    Grid.SetColumn(picker, 1);
        //    rootGrid.Children.Add(picker);

        //    //<Button x:Name="SubmitChanges" 
        //    //        Grid.Row="3" 
        //    //        Grid.Column="3" 
        //    //        HorizontalAlignment="Right" 
        //    //        VerticalAlignment="Top" 
        //    //        Margin="3" 
        //    //        Width="80" 
        //    //        Height="25" 
        //    //        Content="Save" /> 

        //    Button button = new Button();
        //    button.HorizontalAlignment = HorizontalAlignment.Right;
        //    button.VerticalAlignment = VerticalAlignment.Top;
        //    button.Margin = new Thickness(3);
        //    button.Width = 80;
        //    button.Height = 25;
        //    button.Content = "Save";
        //    Grid.SetRow(button, 3);
        //    Grid.SetColumn(button, 1);
        //    rootGrid.Children.Add(button);
           
        //    LayoutRoot.Children.Add(rootGrid);
        //}

        //private TextBlock CreateTextBlock(string text, double height, Thickness margin, int row, int column)
        //{
        //    TextBlock tb = new TextBlock() { Text = text, Height = height, Margin = margin };
        //    Grid.SetColumn(tb, column);
        //    Grid.SetRow(tb, row);

        //    return tb;
        //}

        //private TextBox CreateTextBox(Thickness margin, int row, int column)
        //{
        //    TextBox tb = new TextBox() { Margin = margin };
        //    Grid.SetColumn(tb, column);
        //    Grid.SetRow(tb, row);

        //    return tb;
        //}




    }
}
