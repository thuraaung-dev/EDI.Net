using System;
using System.Collections.Generic;
using System.Text;
using indice.Edi.Serialization;

namespace indice.Edi.Tests.Models
{
    /*public class PaxLst {
        #region header
        public UNB UNB { get; set; }

        public UNG UNG { get; set; }
        public UNH UNH { get; set; }
        public BGM BGM { get; set; }


        #endregion


    }
*/
    public class PaxLst
    {
        public UNB Header { get; set; }
        public UNZ Footer { get; set; }
        public Quote Message { get; set; }
    }

    [EdiMessage]
    public class Quote
    {
     //   public UNG Header { get; set; }
        public UNH Header { get; set; }

        public BGM2 BGM { get; set; }

        public REF REF { get; set; }
     //   public NAD_GR1 NAD_GP1 { get; set; }
        public TDT_GR2 TDT_GR2 { get; set; }
        public NAD_GR4 NAD_GR4 { get; set; }
        public DTM2 DTM2 { get; set; }

        public NAT NAT { get; set; }

   //     
        public REF REF2 { get; set; }
        public ATT ATT { get; set; }
        
        public CNT CNT { get; set; }
        public UNT UNT { get; set; }
    }

    #region header segments

    [EdiSegment, EdiPath("UNB")]
    public class UNB {
        [EdiValue("X(4)", Path = "UNB/0", Mandatory = true)]
        public string Identifier { get; set; } = "UNOA"; //Syntax identifier

        [EdiValue("9(1)", Path = "UNB/0/1", Mandatory = true)]
        public string Version { get; set; } = "4"; //Syntax version number

        /// <summary>
        /// Sender identification
        /// </summary>
        [EdiValue("X(35)", Path = "UNB/1/0", Mandatory = true)]
        public string SenderId { get; set; } = "UKSB";

        [EdiValue("X(4)", Path = "UNB/1/1", Mandatory = false)]
        public string PartnerIDCodeQualifier { get; set; }

        [EdiValue("X(14)", Path = "UNB/1/2", Mandatory = false)]
        public string ReverseRoutingAddress { get; set; }

        [EdiValue("X(35)", Path = "UNB/2/0", Mandatory = true)]
        public string RecipientId { get; set; } = "DLH";


        [EdiValue("X(4)", Path = "UNB/2/1", Mandatory = false)]
        public string ParterIDCode { get; set; }


        [EdiValue("X(14)", Path = "UNB/2/2", Mandatory = false)]
        public string RoutingAddress { get; set; }



        [EdiValue("9(6)", Path = "UNB/3/0", Format = "ddMMyy", Description = "Date of Preparation")]
        [EdiValue("9(4)", Path = "UNB/3/1", Format = "HHmm", Description = "Time or Prep")]
        public DateTime DateOfPreparation { get; set; } = DateTime.ParseExact("110103:0621", "yyMMdd:HHmm",
                                       System.Globalization.CultureInfo.InvariantCulture);

        [EdiValue("X(14)", Path = "UNB/4", Mandatory = true)]
        public string ControlRef { get; set; } = "D8L3H9";

        [EdiValue("9(1)", Path = "UNB/8", Mandatory = false)]
        public int AckRequest { get; set; }
    }


    [EdiSegment, EdiPath("UNG")]
    public class UNG {
        [EdiValue("9(6)", Path = "UNG/0/0", Mandatory = true)]
        public string FGI { get; set; } = "CUSRES";

        /// <summary>
        /// Application sender identification
        /// </summary>
        [EdiValue("X(35)", Path = "UNG/1/0", Mandatory = true)]
        public string ASI { get; set; } = "UKSB";

        [EdiValue("X(35)", Path = "UNG/2/0", Mandatory = true)]
        public string RecipientId2 { get; set; } = "DLH";




        [EdiValue("9(6)", Path = "UNG/3/0", Format = "ddMMyy", Description = "Date of Preparation")]
        [EdiValue("9(4)", Path = "UNG/3/1", Format = "HHmm", Description = "Time or Prep")]
        public DateTime DateOfPreparation2 { get; set; } = DateTime.ParseExact("110103:0621", "yyMMdd:HHmm",
                                 System.Globalization.CultureInfo.InvariantCulture);
        [EdiValue("X(14)", Path = "UNG/4", Mandatory = true)]
        public string ControlRef2 { get; set; } = "D8L3H9";

        [EdiValue("X(2)", Path = "UNG/5")]
        public string ControllingAgency2 { get; set; } = "UN";


        [EdiValue("X(3)", Path = "UNG/6")]
        public string Versions2 { get; set; } = "D";

        [EdiValue("X(3)", Path = "UNG/6/1")]
        public string ReleaseNumber2 { get; set; } = "05B";
    }


    [EdiSegment, EdiPath("UNH")]
    public class UNH {
        [EdiValue("X(14)", Path = "UNH/0/0")]
        public string MessageRef { get; set; } = "1";

        [EdiValue("X(6)", Path = "UNH/1/0")]
        public string MessageType { get; set; } = "CUSRES";

        [EdiValue("X(3)", Path = "UNH/1/1")]
        public string Versions { get; set; } = "D";

        [EdiValue("X(3)", Path = "UNH/1/2")]
        public string ReleaseNumber { get; set; } = "05B";

        [EdiValue("X(2)", Path = "UNH/1/3")]
        public string ControllingAgency { get; set; } = "UN";

        [EdiValue("X(6)", Path = "UNH/1/4")]
        public string AssociationAssignedCode { get; set; } = "IATA";

        [EdiValue("X(35)", Path = "UNH/2/0")]
        public string CommonAccessRef { get; set; }
    }

    
   
    [EdiSegment, EdiPath("BGM")]
    public class BGM2 {
        /// <summary>
        /// 962 Indicates response message
        /// 132 Unsolicited message
        /// 312 Acknowledgement message
        /// </summary>
        [EdiValue("X(3)", Path = "BGM/0/0")]
        public string DocumentNameCode { get; set; }

        [EdiValue("X(35)", Path = "BGM/1/0")]
        public string DocumentIdentifier { get; set; }
    }

    [EdiSegmentGroup("REF")]
    public class REF {
        [EdiValue("X(3)", Path = "REF/0/0", Mandatory = true)]
        public string ReferenceCodeQualifier { get; set; }
        [EdiValue("X(70)", Path = "REF/0/1")]
        public string ReferenceIdentifier { get; set; }
        [EdiValue("X(6)", Path = "REF/0/2")]
        public string DocumentLineIdentifier { get; set; }
        [EdiValue("X(35)", Path = "REF/0/3")]
        public string ReferenceVersionIdentifier { get; set; }
        [EdiValue("X(6)", Path = "REF/0/4")]
        public string RevisionIdentifier { get; set; }
    }
    [EdiSegment, EdiSegmentGroup("COM")]
    public class COM // Level 2 | NAD | GP1
    {
        [EdiValue("X(512)", Path = "COM/0/0", Mandatory = true)]
        public string CommunicationAddressIdentifier { get; set; }

        [EdiValue("X(3)", Path = "COM/0/1", Mandatory = true)]
        public string CommunicationAddresCodeQualifier { get; set; }
    }

    [EdiSegmentGroup("DTM", SequenceEnd = "LOC")]
    public class DTM2 // Level 3 | LOC | GP3
    {
        [EdiValue("X(3)", Path = "DTM/0/0", Mandatory = true)]
        public string DateOrTimeOrPeriodFunctionCodeQualifier { get; set; }
        [EdiValue("X(35)", Path = "DTM/0/1", Mandatory = false)]
        public string DateOrTimeOrPeriodText { get; set; }
        [EdiValue("X(3)", Path = "DTM/0/2", Mandatory = false)]
        public string DateOrTimeOrPeriodFormatCode { get; set; }
    }

    [EdiSegmentGroup("ATT", SequenceEnd = "DTM")]
    public class ATT // Level 2 | NAD | GP4
    {
        [EdiValue("X(3)", Path = "ATT/0/0", Mandatory = true)]
        public string AttributeFunctionCodeQualifier { get; set; }
        [EdiValue("X(17)", Path = "DTM/3/0", Mandatory = false)]
        public string AttributeDescriptionCode { get; set; }
    }

    [EdiSegmentGroup("LOC", SequenceEnd = "NAT")]
    public class LOC2 // Level 2 | NAD | GP4
    {
        [EdiValue("X(125)", Path = "LOC/0/0", Mandatory = true)]
        public string LocationFunctionCodeQualifier { get; set; }

        [EdiValue("X(35)", Path = "COM/1/0", Mandatory = false)]
        public string LocationNameCode { get; set; }
    }
    [EdiSegment, EdiSegmentGroup("NAT")]
    public class NAT // Level 2 | NAD | GP4
    {
        [EdiValue("X(3)", Path = "NAT/0/0", Mandatory = true)]
        public string NationalityCodeQualifier { get; set; }

        [EdiValue("X(3)", Path = "NAT/1/0", Mandatory = false)]
        public string NationalityNameCode { get; set; }
    }
    [EdiSegment, EdiSegmentGroup("CNT")]
    public class CNT // Level 0 
    {
        [EdiValue("X(3)", Path = "CNT/0/0", Mandatory = true)]
        public string ControlTotalTypeCodeQualifier { get; set; }

        [EdiValue("X(18)", Path = "CNT/0/1", Mandatory = true)]
        public string ControlTotalQuantity { get; set; }
        [EdiValue("X(8)", Path = "CNT/0/1", Mandatory = false)]
        public string MeasurementUnitCode { get; set; }
    }
    [EdiSegment, EdiPath("UNT")]
    public class UNT // Level 0 
    {
        [EdiValue("9(10)", Path = "UNT/0/0", Mandatory = true)]
        public string NumberOfSegmentsInAMessage { get; set; }

        [EdiValue("X(14)", Path = "UNT/0/1", Mandatory = true)]
        public string MessageReferenceNumber { get; set; }
    }
    #endregion Header


    #region GR1 | Level 1
  //  [EdiCondition("NAD")]
    [EdiSegment, EdiSegmentGroup("NAD", "COM", SequenceEnd = "TDT")]
    public class NAD_GR1 {
        [EdiValue("X(3)", Path = "NAD/0/0", Mandatory = true)]
        public string PartyFunctionCodeQualifier { get; set; }
        [EdiValue("X(35)", Path = "NAD/1/0", Mandatory = true)]
        public string PartyIdentifier { get; set; }
        [EdiValue("X(17)", Path = "NAD/1/1", Mandatory = false)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "NAD/1/2", Mandatory = false)]
        public string CodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/0", Mandatory = true)]
        public string NameAndAddressDescription { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/0", Mandatory = true)]
        public string PartyName { get; set; }
        [EdiValue("X(35)", Path = "NAD/4/0", Mandatory = true)]
        public string StreetAndNumberOrPostOfficeBoxIdentifier { get; set; }
        public COM[] COM { get; set; }
    }
    #endregion GR1
    #region GR2 | Level 1
    [EdiSegmentGroup("TDT", SequenceEnd = "NAD")]
    public class TDT_GR2 {
        [EdiValue("X(3)", Path = "TDT/0/0", Mandatory = true)]
        public string TransportStageCodeQualifier { get; set; }
        [EdiValue("X(17)", Path = "TDT/0/1")]
        public string MeansOfTransportJourneyIdentifier { get; set; }
        [EdiValue("X(17)", Path = "TDT/4/0")]
        public string CarrierIdentifier { get; set; }
        public LOC_GR3 LOC_GR3 { get; set; }
    }
    #endregion GR2
    #region GR3 | Level 2
    [EdiSegmentGroup("LOC", SequenceEnd = "NAD")]
    public class LOC_GR3 // Level 3 | TDT | GP2
    {
        [EdiValue("X(125)", Path = "LOC/0/0", Mandatory = true)]
        public string LocationFunctionCodeQualifier { get; set; }

        [EdiValue("X(35)", Path = "COM/1/0", Mandatory = false)]
        public string LocationNameCode { get; set; }
        public DTM2 DTM { get; set; }
    }
    #endregion GR3
    #region GR4 | Level 1
  //  [EdiCondition("NAD", Path = "NAD_GR4/0/0")]
    [EdiSegmentGroup("NAD", "DOC", SequenceEnd = "CNT")]
    public class NAD_GR4
    {
        [EdiValue("X(3)", Path = "NAD/0/0", Mandatory = true)]
        public string PartyFunctionCodeQualifier { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/0", Mandatory = true)]
        public string PartyName1 { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/1", Mandatory = false)]
        public string PartyName2 { get; set; }
        [EdiValue("X(35)", Path = "NAD/4/0", Mandatory = true)]
        public string StreetNumberOrPostOfficeboxIdentifier { get; set; }
        [EdiValue("X(35)", Path = "NAD/5/0", Mandatory = true)]
        public string CityName { get; set; }
        [EdiValue("X(9)", Path = "NAD/6/0", Mandatory = false)]
        public string CountrySubEntityNameCode { get; set; }
        [EdiValue("X(17)", Path = "NAD/7/0", Mandatory = false)]
        public string PostalIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "NAD/7/0", Mandatory = false)]
        public string CountryNameCode { get; set; }
        public DOC_GR5 DOC_GR5 { get; set; }
        public LOC2[] LOC { get; set; }
    }
    #endregion GR4
    #region GR5 | Level 2
    [EdiSegmentGroup("DOC", SequenceEnd = "DTN")]
    public class DOC_GR5
    {
        [EdiValue("X(3)", Path = "DOC/0/0", Mandatory = false)]
        public string DocumentNameCode { get; set; }
        [EdiValue("X(17)", Path = "DOC/0/1", Mandatory = false)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "DOC/0/2", Mandatory = false)]
        public string CodeListresponsibleAgencyCode { get; set; }
        [EdiValue("X(35)", Path = "DOC/1/0", Mandatory = false)]
        public string DocumentIdentifier { get; set; }
        public DTM2 DTM2 { get; set; }
        public LOC2 LOC2 { get; set; }
    }
    #endregion GR5
    #region Footer
    [EdiSegment, EdiSegmentGroup("UNZ")]
    public class UNZ
    {
        [EdiValue("9(6)", Path = "UNZ/0")]
        public int TrailerControlCount { get; set; }

        [EdiValue("X(14)", Path = "UNZ/1")]
        public string TrailerControlReference { get; set; }
    }
    #endregion Footer
}
