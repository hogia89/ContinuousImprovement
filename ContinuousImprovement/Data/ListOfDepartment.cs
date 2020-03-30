using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousImprovement.Data
{
    [FirestoreData]
    public class ListOfDepartment
    {
            [FirestoreProperty]
            public string Department { get; set; }
            [FirestoreProperty]
            public string TPU { get; set; }
            [FirestoreProperty]
            public string CostCenter { get; set; }
       
    }
}
