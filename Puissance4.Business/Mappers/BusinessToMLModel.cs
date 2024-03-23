using Puissance4.Business.BusinessObjects;

namespace Puissance4.Business.Mappers
{
    public static class BusinessToMLModel
    {
        public static MLP4Model.ModelInput ToML(this GridBO grid)
        {
            MLP4Model.ModelInput model = new();
            int count = 0;
            for (int i = grid.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < grid.Width; j++)
                {
                    count++;
                    typeof(MLP4Model.ModelInput)
                        .GetProperty($"Pos{count.ToString().PadLeft(2, '0')}")
                        ?.SetValue(model, grid[j, i]);
                }
            }
            return model;
        } 
    }
}
