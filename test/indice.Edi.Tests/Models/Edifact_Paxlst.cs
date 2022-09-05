using System;
using indice.Edi.Serialization;

namespace indice.Edi.Tests.Models
{
   
    public class PaxLst
    {
        public UNB UNB_Header { get; set; }
        public UNG UNG_Header { get; set; }
        public Quote Message { get; set; }
        public UNZ UNZ_Footer { get; set; }        
    }

    [EdiMessage]
    public class Quote
    {
        public UNH UNH_Header { get; set; }

        public BGM2 BGM { get; set; }

        //   public REF REF { get; set; }
        public NAD_GR1 NAD_GP1 { get; set; }
        public TDT_GR2 TDT_GR2 { get; set; }
        public NAD_GR4 NAD_GR4 { get; set; }
        public CNT CNT { get; set; }
        public UNT UNT { get; set; }
        public UNE UNE { get; set; }
    }

    #region header segments
    /// <summary>
    /// Interchange Header
    /// </summary>
    [EdiSegment, EdiPath("UNB")] 
    public class UNB
    {
        [EdiValue("X(4)", Path = "UNB/0", Mandatory = true)]
        public string SyntaxIdentifier { get; set; } = "UNOA"; //Syntax identifier

        [EdiValue("9(1)", Path = "UNB/0/1", Mandatory = true)]
        public string SyntaxVersionNumber { get; set; } = "4"; //Syntax version number

        /// <summary>
        /// Sender identification
        /// </summary>
        [EdiValue("X(35)", Path = "UNB/1/0", Mandatory = true)]
        public string InterchangeSenderIdentification { get; set; } = "UKSB";

        [EdiValue("X(4)", Path = "UNB/1/1", Mandatory = false)]
        public string SenderIdentificationCodeQualifier { get; set; }

        [EdiValue("X(35)", Path = "UNB/1/2", Mandatory = false)]
        public string SenderInterchangeSenderInternalIdentification { get; set; }
        [EdiValue("X(35)", Path = "UNB/1/3", Mandatory = false)]
        public string InterchangeSenderInternalSubIdentification { get; set; }

        [EdiValue("X(35)", Path = "UNB/2/0", Mandatory = true)]
        public string InterchangeRecipientIdentification { get; set; } = "DLH";


        [EdiValue("X(4)", Path = "UNB/2/1", Mandatory = false)]
        public string RecipientIdentificationCodeQualifier { get; set; }

        [EdiValue("X(35)", Path = "UNB/2/2", Mandatory = false)]
        public string InterchangeRecipientInternalIdentification { get; set; }
        [EdiValue("X(35)", Path = "UNB/2/3", Mandatory = false)]
        public string InterchangeRecipientnternalSubIdentification { get; set; }



        [EdiValue("9(8)", Path = "UNB/3/0", Format = "ddMMyy", Description = "Date of Preparation")]
        [EdiValue("9(4)", Path = "UNB/3/1", Format = "HHmm", Description = "Time or Prep")]
        public DateTime DateTimeOfPreparation { get; set; } = DateTime.ParseExact("110103:0621", "yyMMdd:HHmm",
                                       System.Globalization.CultureInfo.InvariantCulture);

        [EdiValue("X(14)", Path = "UNB/4", Mandatory = true)]
        public string InterchangeControlReference { get; set; } = "D8L3H9";
        [EdiValue("X(14)", Path = "UNB/6", Mandatory = false)]
        public string ApplicationReference { get; set; }
        [EdiValue("X(1)", Path = "UNB/7", Mandatory = false)]
        public string ProcessingPriorityCode { get; set; }

        [EdiValue("9(1)", Path = "UNB/8", Mandatory = false)]
        public int AcknowledgementRequest { get; set; }
    }

    /// <summary>
    /// UNG - Functional Group Header
    /// </summary>
    [EdiSegment, EdiPath("UNG")]
    public class UNG
    {
        [EdiValue("X(6)", Path = "UNG/0/0", Mandatory = true)]
        public string FunctionalGroupIdentification { get; set; } = "CUSRES";

        /// <summary>
        /// Application sender identification
        /// </summary>
        [EdiValue("X(35)", Path = "UNG/1/0", Mandatory = true)]
        public string ApplicationSenderIdentification { get; set; } = "UKSB";
        [EdiValue("X(4)", Path = "UNG/1/1", Mandatory = false)]
        public string PartnerIdentificationCodeQualifier { get; set; } = "UKSB";

        [EdiValue("X(35)", Path = "UNG/2/0", Mandatory = true)]
        public string ApplicationRecipientIdentififcation { get; set; } = "DLH";
        [EdiValue("9(6)", Path = "UNG/3/0", Format = "ddMMyy", Description = "Date of Preparation")]
        [EdiValue("9(4)", Path = "UNG/3/1", Format = "HHmm", Description = "Time or Prep")]
        public DateTime DateTimeOfPreparation { get; set; } = DateTime.ParseExact("110103:0621", "yyMMdd:HHmm",
                                 System.Globalization.CultureInfo.InvariantCulture);
        [EdiValue("X(14)", Path = "UNG/4", Mandatory = true)]
        public string FunctionGroupReferenceNumber { get; set; } = "D8L3H9";

        [EdiValue("X(2)", Path = "UNG/5", Mandatory = true)]
        public string ControllingAgency { get; set; } = "UN";


        [EdiValue("X(3)", Path = "UNG/6/0", Mandatory = true)]
        public string MessageTypeVersionNumber { get; set; } = "D";

        [EdiValue("X(3)", Path = "UNG/6/1, Mandatory = true")]
        public string MessageTypeReleaseNumber { get; set; } = "05B";
    }

    /// <summary>
    /// UNH - Message Header
    /// </summary>
    [EdiSegment, EdiPath("UNH")]
    public class UNH
    {
        [EdiValue("X(14)", Path = "UNH/0/0", Mandatory = true)]
        public string MessageReferenceNumber { get; set; } = "1";

        [EdiValue("X(6)", Path = "UNH/1/0", Mandatory = true)]
        public string MessageType { get; set; } = "CUSRES";

        [EdiValue("X(3)", Path = "UNH/1/1", Mandatory = true)]
        public string MessageVersionNumber { get; set; } = "D";

        [EdiValue("X(3)", Path = "UNH/1/2", Mandatory = true)]
        public string MessageReleaseNumber { get; set; } = "05B";

        [EdiValue("X(3)", Path = "UNH/1/3", Mandatory = true)]
        public string ControllingAgencyCoded { get; set; } = "UN";

        [EdiValue("X(6)", Path = "UNH/1/4")]
        public string AssociationAssignedCode { get; set; } = "IATA";

        [EdiValue("X(35)", Path = "UNH/2/0")]
        public string CommonAccessRefence { get; set; }
        [EdiValue("9(2)", Path = "UNH/3/0", Mandatory = true)]
        public int SequenceOfTransfers { get; set; }
        [EdiValue("X(1)", Path = "UNH/3/1", Mandatory = false)]
        public int FirstAndLastTransfer { get; set; }
        [EdiValue("X(14)", Path = "UNH/4/0", Mandatory = true)]
        public int MessageSubsetIdentification { get; set; }
        [EdiValue("X(3)", Path = "UNH/4/1", Mandatory = false)]
        public int MessageSubsetVersionNumber { get; set; }
        [EdiValue("X(3)", Path = "UNH/4/2", Mandatory = false)]
        public int MessageSubsetReleaseNumber { get; set; }
        [EdiValue("X(14)", Path = "UNH/5/0", Mandatory = true)]
        public int MessageImplementationGuidelineIdentification { get; set; }
        [EdiValue("X(3)", Path = "UNH/5/1", Mandatory = false)]
        public int MessageImplementationGuidelineVersionNumber { get; set; }
        [EdiValue("X(3)", Path = "UNH/5/2", Mandatory = false)]
        public int MessageImplementationGuidelineReleaseNumber { get; set; }
        [EdiValue("X(14)", Path = "UNH/6/0", Mandatory = true)]
        public int ScenarioIdentification { get; set; }
        [EdiValue("X(3)", Path = "UNH/6/1", Mandatory = false)]
        public int ScenarioVersionNumber { get; set; }
        [EdiValue("X(3)", Path = "UNH/6/2", Mandatory = false)]
        public int ScenarioReleaseNumber { get; set; }
    }


    /// <summary>
    /// BGM - Beginning of Message
    /// </summary>
    [EdiSegment, EdiPath("BGM")]
    public class BGM2
    {
        /// <summary>
        /// 962 Indicates response message
        /// 132 Unsolicited message
        /// 312 Acknowledgement message
        /// </summary>
        [EdiValue("X(3)", Path = "BGM/0/0")]
        public string DocumentNameCode { get; set; }

        [EdiValue("X(35)", Path = "BGM/1/0")]
        public string DocumentIdentifier { get; set; }
        [EdiValue("X(3)", Path = "BGM/2/0")]
        public string MessageFunctionCode { get; set; }
        [EdiValue("X(3)", Path = "BGM/3/0")]
        public string ResponseTypeCode { get; set; }
    }
    /// <summary>
    /// REF - Reference
    /// </summary>
    [EdiSegment, EdiPath("REF")]
    public class REF
    {
        [EdiValue("X(3)", Path = "REF/0/0", Mandatory = true)]
        public string ReferenceCodeQualifier { get; set; }
        [EdiValue("X(70)", Path = "REF/0/1")]
        public string ReferenceIdentifier { get; set; }
        [EdiValue("X(6)", Path = "REF/0/2")]
        public string DocumentLineIdentifier { get; set; }
        [EdiValue("X(9)", Path = "REF/0/3")]
        public string VersionIdentifier { get; set; }
        [EdiValue("X(6)", Path = "REF/0/4")]
        public string RevisionIdentifier { get; set; }
    }
    /// <summary>
    /// COM - Communication Contact
    /// </summary>
    [EdiSegment, EdiSegmentGroup("COM")]
    public class COM // Level 2 | NAD | GP1
    {
        [EdiValue("X(512)", Path = "COM/0/0", Mandatory = true)]
        public string CommunicationAddressIdentifier { get; set; }

        [EdiValue("X(3)", Path = "COM/0/1", Mandatory = true)]
        public string CommunicationAddresCodeQualifier { get; set; }
    }
    /// <summary>
    /// DTM - Date/Time/Period
    /// </summary>
    [EdiSegment, EdiPath("DTM"), EdiSegmentGroup("LOC", "DOC")]
    public class DTM2 // Level 3 | LOC | GP3
    {
        [EdiValue("X(3)", Path = "DTM/0/0", Mandatory = true)]
        public string DateOrTimeOrPeriodFunctionCodeQualifier { get; set; }
        [EdiValue("X(35)", Path = "DTM/0/1", Mandatory = false)]
        public string DateOrTimeOrPeriodValue { get; set; }
        [EdiValue("X(3)", Path = "DTM/0/2", Mandatory = false)]
        public string DateOrTimeOrPeriodFormatCode { get; set; }
    }
    /// <summary>
    /// ATT - Attribute
    /// </summary>
    [EdiSegment, EdiPath("ATT")]
    public class ATT // Level 2 | NAD | GP4
    {
        [EdiValue("X(3)", Path = "ATT/0/0", Mandatory = true)]
        public string AttributeFunctionCodeQualifier { get; set; }
        [EdiValue("X(3)", Path = "ATT/1/0", Mandatory = true)]
        public string AttributeTypeDescription { get; set; }

        [EdiValue("X(17)", Path = "DTM/2/0", Mandatory = true)]
        public string AttributeDescriptionCode { get; set; }
        [EdiValue("X(17)", Path = "DTM/2/1", Mandatory = true)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "DTM/2/2", Mandatory = true)]
        public string CodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(256)", Path = "DTM/2/3", Mandatory = true)]
        public string AttributeDescription { get; set; }
    }
    /// <summary>
    /// LOC - Place/Location Identification
    /// </summary>
    [EdiSegment, EdiPath("LOC"), EdiSegmentGroup("NAD", "DOC")]
    public class LOC2 // Level 2 | NAD | GP4
    {
        [EdiValue("X(35)", Path = "LOC/1/0", Mandatory = false)]
        public string LocationNameCode { get; set; }
        [EdiValue("X(17)", Path = "LOC/1/1", Mandatory = false)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "LOC/1/2", Mandatory = false)]
        public string CodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(256)", Path = "LOC/1/3", Mandatory = false)]
        public string LocationName { get; set; }
    }
    /// <summary>
    ///  NAT - Nationality
    /// </summary>
    [EdiSegment, EdiPath("NAT")]
    public class NAT // Level 2 | NAD | GP4
    {
        [EdiValue("X(3)", Path = "NAT/0/0", Mandatory = true)]
        public string NationalityCodeQualifier { get; set; }

        [EdiValue("X(3)", Path = "NAT/1/0", Mandatory = false)]
        public string NationalityNameCode { get; set; }
    }
    /// <summary>
    /// CNT - Control Total
    /// </summary>
    [EdiSegment, EdiPath("CNT")]
    //[EdiSegmentGroup("CNT", SequenceEnd = "TDT")]
    
    public class CNT // Level 0 
    {
        [EdiValue("X(3)", Path = "CNT/0/0", Mandatory = true)]
        public string ControlTotalTypeCodeQualifier { get; set; }

        [EdiValue("X(18)", Path = "CNT/0/1", Mandatory = true)]
        public string ControlTotalValue { get; set; }
        [EdiValue("X(3)", Path = "CNT/0/2", Mandatory = false)]
        public string MeasurementUnitCode { get; set; }
    }
    /// <summary>
    /// UNT - Message Trailer
    /// </summary>
    [EdiSegment, EdiPath("UNT")]
    public class UNT // Level 0 
    {
        [EdiValue("9(10)", Path = "UNT/0/0", Mandatory = true)]
        public int NumberOfSegmentsInAMessage { get; set; }

        [EdiValue("X(14)", Path = "UNT/0/1", Mandatory = true)]
        public string MessageReferenceNumber { get; set; }
    }
    /// <summary>
    /// UNE - FUNCTIONAL GROUP TRAILER
    /// </summary>
    [EdiSegment, EdiPath("UNE")]
    public class UNE // Level 0 
    {
        [EdiValue("9(6)", Path = "UNE/0/0", Mandatory = true)]
        public int NumberOfMessages { get; set; }

        [EdiValue("X(14)", Path = "UNE/0/1", Mandatory = true)]
        public string ApplicationSenderIdentification { get; set; }
    }
    #endregion Header


    #region GR1 | Level 1
    //  [EdiCondition("NAD")]
    /// <summary>
    /// NAD - Name and Address
    /// </summary>
    [EdiSegmentGroup("NAD", SequenceEnd = "TDT")]
    [EdiCondition("MS", CheckFor = EdiConditionCheckType.Equal, Path = "NAD/0/0")]
    //  [EdiSegment, EdiPath("NAD")]
    public class NAD_GR1
    {
        [EdiValue("X(3)", Path = "NAD/0/0", Mandatory = true)]
        public string PartyFunctionCodeQualifier { get; set; }
        [EdiValue("X(35)", Path = "NAD/1/0", Mandatory = true)]
        public string PartyIdentifier { get; set; }
        [EdiValue("X(17)", Path = "NAD/1/1", Mandatory = false)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "NAD/1/2", Mandatory = false)]
        public string CodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/0", Mandatory = false)]
        public string NameAndAddressDescription1 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/1", Mandatory = false)]
        public string NameAndAddressDescription2 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/2", Mandatory = false)]
        public string NameAndAddressDescription3 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/3", Mandatory = false)]
        public string NameAndAddressDescription4 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/4", Mandatory = false)]
        public string NameAndAddressDescription5 { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/0", Mandatory = true)]
        public string PartyName1 { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/1", Mandatory = true)]
        public string PartyName2 { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/2", Mandatory = true)]
        public string PartyName3 { get; set; }
        [EdiValue("X(35)", Path = "NAD/4/0", Mandatory = false)]
        public string StreetAndNumberOrPostOfficeBoxIdentifier1 { get; set; }
        [EdiValue("X(35)", Path = "NAD/4/1", Mandatory = false)]
        public string StreetAndNumberOrPostOfficeBoxIdentifier2 { get; set; }
        [EdiValue("X(35)", Path = "NAD/4/2", Mandatory = false)]
        public string StreetAndNumberOrPostOfficeBoxIdentifier3 { get; set; }
        [EdiValue("X(35)", Path = "NAD/4/3", Mandatory = false)]
        public string StreetAndNumberOrPostOfficeBoxIdentifier4 { get; set; }
        [EdiValue("X(35)", Path = "NAD/5/0", Mandatory = false)]
        public string CityName { get; set; }
        [EdiValue("X(9)", Path = "NAD/6/0", Mandatory = false)]
        public string CountrySubEntryNameCode { get; set; }
        [EdiValue("X(17)", Path = "NAD/7/0", Mandatory = false)]
        public string PostalIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "NAD/8/0", Mandatory = false)]
        public string CountryNameCode { get; set; }

        public COM[] COM { get; set; }
    }
    #endregion GR1
    #region GR2 | Level 1
    /// <summary>
    /// TDT - Details of Transport
    /// </summary>
    [EdiSegmentGroup("TDT", SequenceEnd = "NAD")]
    public class TDT_GR2
    {
        [EdiValue("X(3)", Path = "TDT/0/0", Mandatory = true)]
        public string TransportStageCodeQualifier { get; set; }
        [EdiValue("X(17)", Path = "TDT/1/0")]
        public string MeansOfTransportJourneyIdentifier { get; set; }
        [EdiValue("X(3)", Path = "TDT/2/0")]
        public string TransportModeNameCode { get; set; }
        [EdiValue("X(17)", Path = "TDT/2/1")]
        public string TransportModeName { get; set; }
        [EdiValue("X(8)", Path = "TDT/3/0")]
        public string TransportMeansDescriptionCode { get; set; }
        [EdiValue("X(17)", Path = "TDT/3/1")]
        public string TransportMeansDescription { get; set; }
        [EdiValue("X(17)", Path = "TDT/4/0")]
        public string CarrierIdentifier { get; set; }
        [EdiValue("X(17)", Path = "TDT/4/1")]
        public string CarrierListIdentificatonCode { get; set; }
        [EdiValue("X(17)", Path = "TDT/4/2")]
        public string CarrierListResponsibleAgencyCode { get; set; }
        [EdiValue("X(3)", Path = "TDT/5/0")]
        public string TransitDirectionIndicatorCode { get; set; }
        [EdiValue("X(3)", Path = "TDT/6/0", Mandatory = true)]
        public string ExcessTransportationReasonCode { get; set; }
        [EdiValue("X(3)", Path = "TDT/6/1", Mandatory = true)]
        public string ExcessTransportationResponsibilityCode { get; set; }
        [EdiValue("X(17)", Path = "TDT/6/2", Mandatory = false)]
        public string CustomerShipmentAuthorisationIdentifier { get; set; }
        [EdiValue("X(9)", Path = "TDT/7/0", Mandatory = false)]
        public string TransportMeansIdentificationNameIdentifier { get; set; }
        [EdiValue("X(17)", Path = "TDT/7/1", Mandatory = false)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "TDT/7/2", Mandatory = false)]
        public string CodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(35)", Path = "TDT/7/3", Mandatory = false)]
        public string TransportMeansIdentificationName { get; set; }
        [EdiValue("X(3)", Path = "TDT/7/4", Mandatory = false)]
        public string TransportMeansNationalityCode { get; set; }
        [EdiValue("X(3)", Path = "TDT/8/0", Mandatory = false)]
        public string TransportMeansOwnershipIndicatorCode { get; set; }
        public LOC_GR3[] LOC_GR3 { get; set; }
    }
    #endregion GR2
    #region GR3 | Level 2
    /// <summary>
    /// LOC - Place/Location Identification
    /// </summary>
    [EdiSegmentGroup("LOC", SequenceEnd = "NAD")]
    public class LOC_GR3 // Level 3 | TDT | GP2
    {
        [EdiValue("X(3)", Path = "LOC/0/0", Mandatory = true)]
        public string LocationFunctionCodeQualifier { get; set; }
        [EdiValue("X(35)", Path = "LOC/1/0", Mandatory = false)]
        public string LocationNameCode { get; set; }
        [EdiValue("X(17)", Path = "LOC/1/1", Mandatory = false)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "LOC/1/2", Mandatory = false)]
        public string CodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(256)", Path = "LOC/1/3", Mandatory = false)]
        public string LocationName { get; set; }
        [EdiValue("X(25)", Path = "LOC/2/0", Mandatory = false)]
        public string FirstRelatedLocationNameCode { get; set; }
        [EdiValue("X(17)", Path = "LOC/2/1", Mandatory = false)]
        public string FirstRelatedCodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "LOC/2/2", Mandatory = false)]
        public string FirstRelatedCodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(70)", Path = "LOC/2/3", Mandatory = false)]
        public string FirstRelatedLocationName { get; set; }
        [EdiValue("X(25)", Path = "LOC/3/0", Mandatory = false)]
        public string SecondRelatedLocationNameCode { get; set; }
        [EdiValue("X(17)", Path = "LOC/3/1", Mandatory = false)]
        public string SecondRelatedCodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "LOC/3/2", Mandatory = false)]
        public string SecondRelatedCodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(70)", Path = "LOC/3/3", Mandatory = false)]
        public string SecondRelatedLocationName { get; set; }
        public DTM2 DTM { get; set; }
    }
    #endregion GR3
    /// <summary>
    /// NAD - Name and Address
    /// </summary>
    #region GR4 | Level 1
    //  [EdiCondition("NAD", Path = "NAD_GR4/0/0")]
    [EdiSegmentGroup("NAD", SequenceEnd = "CNT")]
    [EdiCondition("FL", CheckFor = EdiConditionCheckType.Equal, Path = "NAD/0/0")]
    public class NAD_GR4
    {
        [EdiValue("X(3)", Path = "NAD/0/0", Mandatory = true)]
        public string PartyFunctionCodeQualifier { get; set; }
        [EdiValue("X(35)", Path = "NAD/1/0", Mandatory = false)]
        public string PartyIdentifier { get; set; }
        [EdiValue("X(17)", Path = "NAD/1/1", Mandatory = false)]
        public string CodeListIdentificationCode { get; set; }
        [EdiValue("X(3)", Path = "NAD/1/2", Mandatory = false)]
        public string CodeListResponsibleAgencyCode { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/0", Mandatory = false)]
        public string NameAndAddressDescription1 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/1", Mandatory = false)]
        public string NameAndAddressDescription2 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/2", Mandatory = false)]
        public string NameAndAddressDescription3 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/3", Mandatory = false)]
        public string NameAndAddressDescription4 { get; set; }
        [EdiValue("X(35)", Path = "NAD/2/4", Mandatory = false)]
        public string NameAndAddressDescription5 { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/0", Mandatory = true)]
        public string PartyName1 { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/1", Mandatory = false)]
        public string PartyName2 { get; set; }
        [EdiValue("X(35)", Path = "NAD/3/2", Mandatory = false)]
        public string PartyName3 { get; set; }
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
        public ATT[] ATT { get; set; }
        public DTM2[] DTM { get; set; }
        public LOC2[] LOC { get; set; }
        public NAT[] NAT { get; set; }
        public REF[] REF { get; set; }
        public DOC_GR5[] DOC_GR5 { get; set; }
    }
    #endregion GR4
    #region GR5 | Level 2
    /// <summary>
    /// DOC - Document/Message Details
    /// </summary>
    [EdiSegmentGroup("DOC", SequenceEnd = "CNT")]
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
        public DTM2[] DTM { get; set; }
        public LOC2[] LOC { get; set; }
    }
    #endregion GR5
    #region Footer
    /// <summary>
    /// UNZ - Interchange Trailer
    /// </summary>
    [EdiSegment, EdiPath("UNZ")]
    public class UNZ
    {
        [EdiValue("9(6)", Path = "UNZ/0")]
        public int InterchangeControlCount { get; set; }

        [EdiValue("X(14)", Path = "UNZ/1")]
        public string InterchangeControlReference { get; set; }
    }
    #endregion Footer
}
