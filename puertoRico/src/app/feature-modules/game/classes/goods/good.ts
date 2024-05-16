import { coffee, corn, indigo, Produce, sugar, tobacco } from "./goodTypes";

export class Good{
    constructor(type:Produce){
        this.type = type;
    }
    type:Produce;
}
