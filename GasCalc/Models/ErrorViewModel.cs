// <copyright file="ErrorViewModel.cs" company="MAudu Productions">
// Copyright (c) MAudu Productions. All rights reserved.
// </copyright>

namespace GasCalc.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}