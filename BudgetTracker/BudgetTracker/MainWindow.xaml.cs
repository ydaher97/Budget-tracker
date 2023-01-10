using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Budget : INotifyPropertyChanged
    {
        

        // fields
        private List<Transaction> transactions;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // properties
        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
            set
            {
                balance = value;
                OnPropertyChanged("Balance");
            }
        }
        public decimal TotalIncome { get; private set; }
        public decimal TotalExpenses { get; private set; }

        public List<Transaction> Transactions
        {
            get { return transactions; }
            set
            {
                transactions = value;
                OnPropertyChanged("Transactions");
            }
        }

        // constructor
        public Budget()
        {
            transactions = new List<Transaction>();
            
        }

        //methods
        public void SubtractFromBalance(decimal amount)
        {
            Balance -= amount;
        }



        public void Save(string filename)
        {
            // Create a string builder to store the transactions
            StringBuilder sb = new StringBuilder();

            // Iterate through the transactions and add them to the string builder
            foreach (Transaction t in transactions)
            {
                string transactionString = t.Type + Environment.NewLine + t.Amount + Environment.NewLine + t.Date + Environment.NewLine;
                sb.Append(transactionString);
            }

            // Write the contents of the string builder to the file
            File.WriteAllText(filename, sb.ToString());

        }

        public void Load(string filename)
        {
            if (File.Exists(filename)) 
            { 
                string[] lines = File.ReadAllLines(filename);
                for (int i = 0; i < lines.Length; i += 3)
                {
                    string type = lines[i];
                    decimal amount = decimal.Parse(lines[i + 1]);
                    DateTime date = DateTime.Parse(lines[i + 2]);

                    if (type == "Income")
                    {
                        AddIncome(amount);
                    
                    }
                    else if (type == "Expense")
                    {
                        AddExpense(amount);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid transaction type");
                    }
                }
            }
            else
            {
                // If the file does not exist, create a new empty budget
                transactions = new List<Transaction>();
                Balance = 0;
                File.Create(filename);
            }
        }
       

        public void AddIncome(decimal amount)
        {
            Balance += amount;
            TotalIncome += amount;
            transactions.Add(new Transaction("Income", amount));
            OnPropertyChanged("TotalIncome");

        }

        public void AddExpense(decimal amount)
        {
            Balance -= amount;
            TotalExpenses += amount;
            transactions.Add(new Transaction("Expense", amount));
            OnPropertyChanged("TotalExpenses");
        }

    }

    public class Transaction
    {
        // properties
        public string Type { get; set; }
        public decimal Amount { get;  set; }
        public DateTime Date { get; set; }
        // constructor
        public Transaction(string type, decimal amount)
        {
            if (type != "Income" && type != "Expense")
            {
                throw new ArgumentException("Invalid transaction type. Type must be 'Income' or 'Expense'.");
            }
            Type = type;
            Amount = amount;
            Date = DateTime.Now;


        }
    }

    public partial class MainWindow : Window
    {
        private Budget budget;
       

        public MainWindow()
        {
            InitializeComponent();

            budget = new Budget();
            DataContext = budget;
            
            


            budget.Load("budget.txt");
            
        }

        
        private void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            
            // Read the transaction type and amount from the text boxes
           
            string transactionType = TransactionTypeTextBox.Text;
            decimal transactionAmount = decimal.Parse(TransactionAmountTextBox.Text);
           

            // Call the appropriate method in the Budget class based on the transaction type
            if (transactionType == "Income")
            {
                budget.AddIncome(transactionAmount);
            }
            else if (transactionType == "Expense")
            {
                budget.AddExpense(transactionAmount);
            }
            else
            {
                // Handle invalid transaction type
            }


            budget.Save("budget.txt");
           

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
           // budget.Load("budget.txt");
        }

        private void DeleteTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            //Transaction transaction = (Transaction)((Button)sender).DataContext;
            //budget.Transactions.Remove(transaction);
            //budget.SubtractFromBalance(transaction.Amount);

            // Get the selected transaction from the list view
            Transaction selectedTransaction = (Transaction)((Button)sender).DataContext;

            // Remove the transaction from the list
            budget.Transactions.Remove(selectedTransaction);

            // Update the balance
            if (selectedTransaction.Type == "Income")
            {
                // Subtract the transaction amount from the balance
                budget.SubtractFromBalance(selectedTransaction.Amount);
            }
            else
            {
                // Add the transaction amount to the balance
                budget.Balance += selectedTransaction.Amount;
            }

            // Update the total income and total expenses
           budget.TotalIncome

            // Update the balance label
            OnPropertyChanged("Balance");
        }



    }
}
