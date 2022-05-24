using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VI.AOD.Preprocessing.Preprocessing.SleeperObjectClassification;
using VI.AOD.Preprocessing.Services;
using VI.AOD.Preprocessing.SleeperObjectClassification;

namespace VI.AOD.Preprocessing.Services.SleeperObjectClassification
{
    public class SleeperObjectClassificationFactory
    {
        public SleeperObjectClassificationPreprocessingService CreatePreprocessingService(EPreprocessingStage preprocessingStage)
        {
            switch(preprocessingStage)
            {
                case (EPreprocessingStage.FirstStage): 
                    return new SleeperObjectClassificationPreprocessingService(new FirstStagePreprocessing());
                    
                case (EPreprocessingStage.SecondStage):
                    return new SleeperObjectClassificationPreprocessingService(new SecondStagePreprocessing());

                case (EPreprocessingStage.Common):
                    return new SleeperObjectClassificationPreprocessingService(new CommonPreprocessing());
                  
                default: 
                    return new SleeperObjectClassificationPreprocessingService(new FirstStagePreprocessing());

            }

        }
    }
}
