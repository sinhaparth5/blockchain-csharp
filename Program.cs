// See https://aka.ms/new-console-template for more information

using System;
using Blockchain;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Input a miner address: ");
        string minerAddress = Console.ReadLine();
        
        Console.WriteLine("Difficulty: ");
        string difficultyInput = Console.ReadLine();
        if (!uint.TryParse(difficultyInput, out uint difficulty))
        {
            Console.WriteLine("We need an integer for difficulty!");
            return;
        }
        
        Console.WriteLine("Generating genesis block!");
        Chain chain = new Chain(minerAddress.Trim(), difficulty);

        while (true)
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. New transaction");
            Console.WriteLine("2. Mine Block");
            Console.WriteLine("3. Chain difficulty");
            Console.WriteLine("4. Change reward");
            Console.WriteLine("0. Exit");
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "0":
                    Console.WriteLine("Exiting");
                    Environment.Exit(0);
                    break;
                case "1":
                    Console.WriteLine("Enter sender address:");
                    string sender = Console.ReadLine();
                    Console.WriteLine("Enter receiver address:");
                    string receiver = Console.ReadLine();
                    Console.WriteLine("Enter amount:");
                    string amountInput = Console.ReadLine();

                    if (float.TryParse(amountInput, out float amount))
                    {
                        bool res = chain.NewTransaction(sender.Trim(), receiver.Trim(), amount);
                        Console.WriteLine(res ? "Transaction added" : "Transaction failed");
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount");
                    }
                    break;
                case "2":
                    Console.WriteLine("Generating block");
                    bool success = chain.GenerateNewBlock();
                    Console.WriteLine(success ? "Block generated successfully" : "Block generation failed");
                    break;
                case "3":
                    Console.WriteLine("Please enter new difficulty:");
                    string newDiffInput = Console.ReadLine();
                    if (uint.TryParse(newDiffInput, out uint newDiff))
                    {
                        bool res = chain.UpdateDifficulty(newDiff);
                        Console.WriteLine(res ? "Updated difficulty" : "Failed update");
                    }
                    else
                    {
                        Console.WriteLine("Invalid difficulty input");
                    }
                    break;
                case "4":
                    Console.WriteLine("Enter new reward:");
                    string newRewardInput = Console.ReadLine();
                    if (float.TryParse(newRewardInput, out float newReward))
                    {
                        bool res = chain.UpdateReward(newReward);
                        Console.WriteLine(res ? "Updated reward" : "Failed update");
                    }
                    else
                    {
                        Console.WriteLine("Invalid reward input");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again");
                    break;
            }
        }
    }
}