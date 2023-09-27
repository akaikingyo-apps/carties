'use server'

import { Auction, PagedResult } from "@/types";

export async function getData(query:string): Promise<PagedResult<Auction>> {
    console.log(`getData(): pagenumber:${query}`)
    const res = await fetch(`http://localhost:6001/search${query}`);
    if (!res.ok) {
        throw new Error('fail to fetch data');
    }
    return res.json(); // to transform body to json object
}
