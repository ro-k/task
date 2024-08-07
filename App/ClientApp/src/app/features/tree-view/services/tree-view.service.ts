import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {TreeView} from "../../../core/models/tree-view";


@Injectable({
  providedIn: 'root'
})
export class TreeViewService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {  }

  getTreeView(): Observable<TreeView> {
    return this.http.get<TreeView>(this.baseUrl + 'api/treeview');
  }
}
