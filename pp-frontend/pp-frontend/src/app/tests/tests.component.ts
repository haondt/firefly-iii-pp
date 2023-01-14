import { Component } from '@angular/core';
import { iif, map, mergeMap, of } from 'rxjs';
import { FolderContentModel } from '../models/FolderContent';
import { TestModel } from '../models/Test';
import { FolderModel } from '../models/Folder';
import { MongoDbService } from '../services/MongoDb';
import { TestBuilderService } from '../services/TestBuilder';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { NestedTreeControl } from '@angular/cdk/tree';
import { TreeNode } from '../models/TreeNode';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { CheckModel } from '../models/Check';

@Component({
  selector: 'app-tests',
  templateUrl: './tests.component.html',
  styleUrls: ['./tests.component.scss'],
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

  addTestNode(parent: any) {
    if (parent instanceof FolderModel){
      parent.items.push(new TestModel());
      const d = this.dataSource.data;
      this.dataSource.data = [];
      this.dataSource.data = d;
      if (!this.treeControl.isExpanded(parent)){
        this.treeControl.expand(parent);
      }
    } else {
      throw new TypeError(`Expected object of type FolderModel but received ${parent.constructor.name}`);
    }
  }

  addFolderNode(parent: any) {
    if (parent instanceof FolderModel){
      parent.items.push(new FolderModel());
      const d = this.dataSource.data;
      this.dataSource.data = [];
      this.dataSource.data = d;
      if (!this.treeControl.isExpanded(parent)){
        this.treeControl.expand(parent);
      }
    } else {
      throw new TypeError(`Expected object of type FolderModel but received ${parent.constructor.name}`);
    }
  }

  isFolderNode(node: any){
    return node instanceof FolderModel;
  }

  isTestNode(node: any){
    return node instanceof TestModel;
  }

  isExpanded(node: TreeNode) {
    if ('expanded' in node.meta) {
      if (node.meta['expanded'] === true) {
        return true;
      }
    }
    return false;
  }

  expandNode(node: TreeNode) {
    if(this.isTestNode(node)) {
      node.meta['expanded'] = true;
    }
  }

  collapseNode(node: TreeNode) {
    if(this.isTestNode(node)) {
      node.meta['expanded'] = false;
    }
  }

  toggleExpanded(node: TreeNode) {
    if (this.isExpanded(node)) {
      this.collapseNode(node);
    } else {
      this.expandNode(node);
    }
  }

  isChecksExpanded(node: TestModel) {
    if ('checks_expanded' in node.meta) {
      if (node.meta['checks_expanded'] === true) {
        return true;
      }
    }
    return false;
  }

  toggleChecksExpanded(node: TestModel) {
    if (this.isCasesExpanded(node)) {
      node.meta['checks_expanded'] = false;
    } else {
      node.meta['checks_expanded'] = true;
    }
  }


  isCasesExpanded(node: TestModel) {
    if ('cases_expanded' in node.meta) {
      if (node.meta['cases_expanded'] === true) {
        return true;
      }
    }
    return false;
  }

  toggleCasesExpanded(node: TestModel) {
    if (this.isCasesExpanded(node)) {
      node.meta['cases_expanded'] = false;
    } else {
      node.meta['cases_expanded'] = true;
    }
  }


  // TODO: make sure you cant put a folder in its own subfolder (use the material thing that pops up at the bottom of the screen)

}
