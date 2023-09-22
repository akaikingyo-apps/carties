'use server'

import { Auction, PagedResult } from "@/types";

export async function getData(pageNumber:number,pageSize:number): Promise<PagedResult<Auction>> {
    console.log(`getData(): pagenumber:${pageNumber} pagesize:${pageSize}`)
    const res = await fetch(`http://localhost:6001/search?pageSize=${pageSize}&pageNumber=${pageNumber}`);
    if (!res.ok) {
        throw new Error('fail to fetch data');
    }
    return res.json(); // to transform body to json object
}