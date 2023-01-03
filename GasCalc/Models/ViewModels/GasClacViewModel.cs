// <copyright file="GasClacViewModel.cs" company="MAudu Productions">
// Copyright (c) MAudu Productions. All rights reserved.
// </copyright>

namespace GasCalc.Models.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The Gas Calc ViewModel.
    /// </summary>
    public class GasClacViewModel
    {
        [Required]
        public double TankSize { get; set; }

        [Required]
        [DisplayName("Approximately amount left in tank?")]
        public int CurrentGasAmount { get; set; }

        [Required]
        [DisplayName("($) Amount you wish to spend")]
        //[DataType(DataType.Currency)]
        //[DisplayFormat(DataFormatString = "{0:#,###.00}", ApplyFormatInEditMode = true)]
        public double AmountToSpend { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        /// <value>
        /// The zip code.
        /// </value>
        [DisplayName("ZipCode")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        [Required(ErrorMessage = "Please enter a valid zipcode")]
        public int Zipcode { get; set; }

        public List<GasCalcResult>? GasCalcResults { get; set; }
    }
}
