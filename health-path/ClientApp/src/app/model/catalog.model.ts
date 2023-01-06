
interface NaturalProduct {
    readonly licenceNo: string;
    readonly productName: string;
    readonly companyName: string;
    readonly active: boolean;
    readonly purposes: readonly string[];
    readonly ProductRoute: string;
}

export { NaturalProduct };
