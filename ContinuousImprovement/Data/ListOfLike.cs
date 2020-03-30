using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousImprovement.Data
{
    [FirestoreData]
    public class ListOfLike
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string userName { get; set; }
    }
}
