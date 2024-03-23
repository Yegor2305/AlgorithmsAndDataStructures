using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    struct Project(DateTime startDate, DateTime endDate, float profit)
    {
        public DateTime startDate = startDate;
        public DateTime endDate = endDate;
        public float profit = profit;

        public override string ToString()
        {
            return $"{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}, profit - {profit}";
        }
    }

    internal class InvestorProblem
    {

        private List<Project> inputProjects;
        private List<Project> selectedProjects;

        public InvestorProblem()
        {
            inputProjects = [];
            selectedProjects = [];
        }

        public InvestorProblem(List<Project> inputProjects)
        {
            this.inputProjects = inputProjects;
            selectedProjects = [];
        }

        public void AddProject(Project project)
        {
            inputProjects.Add(project);
        }

        public void SelectProjects()
        {
            if (inputProjects.Count == 0) return;

            inputProjects.Sort((x, y) => y.profit.CompareTo(x.profit));
            selectedProjects.Add(inputProjects[0]);

            foreach (Project project in inputProjects)
            {
                if (project.startDate >= selectedProjects.Last().endDate)
                {
                    selectedProjects.Add(project);
                }
            }
        }

        public void Clear()
        {
            inputProjects.Clear();
            selectedProjects.Clear();
        }

        public string GetSelectedProjects()
        {
            return string.Join('\n', selectedProjects.Select(item => item.ToString()));
        }

        public string GetInputProjects()
        {
            return string.Join('\n', inputProjects.Select(item => item.ToString()));
        }

        public string GetProfit()
        {
            if (selectedProjects.Count == 0) return "";
            return $"General profit: {selectedProjects.Sum(item => item.profit)}";
        }
    }
}
