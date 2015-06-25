using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IOperatorSettingRepository
    {
        void Insert(Entities.OperatorSetting operatorSetting);

        IEnumerable<Entities.OperatorSetting> GetAll();

        void Update(Entities.OperatorSetting operatorSetting);

        Entities.OperatorSetting GetByOperator(string name);
    }
}
