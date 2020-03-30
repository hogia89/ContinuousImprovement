using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousImprovement.Data
{
    [FirestoreData]
    public class Suggestion
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public String ownerSuggestion { get; set; }
        [FirestoreProperty]
        public String ownerCode { get; set; }
        [FirestoreProperty]
        public String ownerDept { get; set; }
        [FirestoreProperty]
        public Timestamp submitDate { get; set; }
        [FirestoreProperty]
        public String status { get; set; }
        [FirestoreProperty]
        public String currentStatus { get; set; }
        [FirestoreProperty]
        public String suggestionAction { get; set; }
        [FirestoreProperty]
        public String expectedBenefit { get; set; }
        [FirestoreProperty]
        public String deptRecSug { get; set; }
        [FirestoreProperty]
        public String email { get; set; }
        [FirestoreProperty]
        public Timestamp approveDt { get; set; }
        [FirestoreProperty]
        public String approveSuggBy { get; set; }
        [FirestoreProperty]
        public String ownerAction { get; set; }
        [FirestoreProperty]
        public Timestamp assignDt { get; set; }
        [FirestoreProperty]
        public Timestamp planFinishActionDt { get; set; }
        [FirestoreProperty]
        public String imageUri { get; set; }
        public int CountLike { get; set; }
        public bool like { get; set; }
    }
}
