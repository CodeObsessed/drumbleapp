using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;

namespace DrumbleApp.Shared.Converters
{
    public class OperatorModeImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Guid operatorId = Guid.Parse(value.ToString());

            return null;// Operators.SingleOrDefault(x => x.Id == operatorId).Mode.ModeImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private IUnitOfWork unitOfWork;
        private IUnitOfWork UnitOfWork
        {
            get
            {
                if (unitOfWork == null)
                    unitOfWork = SimpleIoc.Default.GetInstance<IUnitOfWork>();

                return unitOfWork;
            }
        }

        private List<PublicTransportOperator> operators;
        private List<PublicTransportOperator> Operators
        {
            get
            {
                if (operators == null)
                    operators = UnitOfWork.PublicTransportOperatorRepository.GetAll().ToList();

                return operators;
            }
        }
    }
}
