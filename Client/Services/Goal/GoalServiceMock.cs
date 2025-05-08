using Core;

namespace Client
{

    public class GoalServiceMock : IGoal
    {

        private List<Goal> _goals = new(){
            
            new Goal
            {
                Id = 101,
                Type = "Kursus",
                Title = "Velkommen til og introduktion til kollegaer",
                Description = "Introduktion til arbejdspladsen og møde med kollegaer",
                Status = "Template",
                Semester = 1,
                SortOrder = 1,
                DeadLineAt = DateTime.Now.AddDays(30)
            },
            new Goal
            {
                Id = 102,
                Type = "Kompetence",
                Title = "Neuroinstructure",
                Description = "Grundlæggende forståelse af neurogastronomi",
                Status = "Template",
                Semester = 1,
                SortOrder = 2,
                DeadLineAt = DateTime.Now.AddDays(60)
            }
        };
        
        public async Task<Goal> GetGoalByGoalId(int goalId)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == goalId);
            return goal;
        }
    }

}