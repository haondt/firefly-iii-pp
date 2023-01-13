import { Component } from '@angular/core';
import { iif, map, mergeMap, of } from 'rxjs';
import { FolderContentModel } from '../models/FolderContent';
import { TestModel } from '../models/Test';
import { MongoDbService } from '../services/MongoDb';
import { TestBuilderService } from '../services/TestBuilder';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { NestedTreeControl } from '@angular/cdk/tree';
import { TreeNode } from '../models/TreeNode';
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-tests',
  templateUrl: './tests.component.html',
  styleUrls: ['./tests.component.scss'],
  animations: [
    trigger('slideVertical', [
      state('*', style({height:0})),
      state('show', style({height:'*'})),
      transition('* => *', [animate('400ms cubic-bezier(0.25,0.8,0.25,1)')])
    ])
  ]
})
export class TestsComponent {
  test_id = '0';

  treeControl = new NestedTreeControl<FolderContentModel>(m => m.items);
  dataSource = new MatTreeNestedDataSource<FolderContentModel>();

  constructor(private testBuilder: TestBuilderService, private mongo: MongoDbService) {
    this.loadTestsFromMongo();
  }

  hasChild = (_: number, node: TreeNode) => !!node.items && node.items.length > 0;

  loadTestsFromMongo() {
    this.mongo.getTestData(this.test_id)
      .pipe(map(d => d
        ? this.testBuilder.build(d)
        : this.testBuilder.buildSample()
      ))
      .subscribe(t => this.dataSource.data = t);
  }

  saveTestsToMongo() {
    console.log(this.dataSource.data);
    //this.mongo.setTestData(this.test_id, this.tests);
  }



}
