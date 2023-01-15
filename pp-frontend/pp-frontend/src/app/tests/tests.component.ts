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
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-tests',
  templateUrl: './tests.component.html',
  styleUrls: ['./tests.component.scss'],
})
export class TestsComponent {
  test_id = '0';

  treeControl = new NestedTreeControl<FolderContentModel>(m => m.items);
  dataSource = new MatTreeNestedDataSource<FolderContentModel>();
  hovered: TreeNode | null = null;

  constructor(private testBuilder: TestBuilderService, private mongo: MongoDbService, private snackBar: MatSnackBar) {
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

  reloadData() {
    const d = this.dataSource.data;
    this.dataSource.data = [];
    this.dataSource.data = d;
  }

  addTestNode(parent: any) {
    if (parent instanceof FolderModel){
      parent.items.unshift(new TestModel());
      this.reloadData();
      if (!this.treeControl.isExpanded(parent)){
        this.treeControl.expand(parent);
      }
    } else {
      throw new TypeError(`Expected object of type FolderModel but received ${parent.constructor.name}`);
    }
  }

  addFolderNode(parent: any) {
    if (parent instanceof FolderModel){
      parent.items.unshift(new FolderModel());
      this.reloadData();
      if (!this.treeControl.isExpanded(parent)){
        this.treeControl.expand(parent);
      }
    } else {
      throw new TypeError(`Expected object of type FolderModel but received ${parent.constructor.name}`);
    }
  }

  addTestNodeToRoot() {
    this.dataSource.data.unshift(new TestModel());
    this.reloadData();
  }

  addFolderNodeToRoot() {
    this.dataSource.data.unshift(new FolderModel());
    this.reloadData();
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


  mouseEnter(node: TreeNode) {
    this.hovered = node;
  }

  mouseLeave(node: TreeNode) {
    this.hovered = null;
  }

  drop(node: FolderContentModel) {
    if (this.hovered === null
      || this.hovered === node
      || !this.isFolderNode(this.hovered)) {
      return;
    }

    if (this.isDescendantOf(node, this.hovered)) {
      this.snackBar.open('The folder cannot be moved into its own subfolder.', 'Dismiss', {
        duration: 3000
      });
      return;
    }

    if (!this.removeFromParent(node)){
      throw new Error(`Unable to remove node ${node.name} from its parent!`);
    }

    if (this.hovered.items === null || this.hovered.items === undefined) {
      this.hovered.items = [];
    }

    this.hovered.items.unshift(node);
    this.reloadData();
  }

  isDescendantOf(parent: TreeNode, node: TreeNode) {
    if (parent.items === null || parent.items === undefined || parent.items.length === 0) {
      return false;
    }

    for(let child of parent.items) {
      if (node == child || this.isDescendantOf(child, node)) {
        return true;
      }
    }

    return false;
  }

  removeFromParent(node: FolderContentModel) {
    if (this.dataSource.data.length == 0) {
      return false;
    }

    for (let i=0; i < this.dataSource.data.length; i++) {
      let ancestor = this.dataSource.data[i];
      if (ancestor == node) {
        this.dataSource.data.splice(i, 1);
        return true;
      }
      if (this._removeFromParent(ancestor, node)) {
        return true;
      }
    }

    return false;
  }

  _removeFromParent(ancestor: FolderContentModel, node: TreeNode) {
    if (ancestor.items === null
      || ancestor.items === undefined
      || ancestor.items.length === 0) {
        return false;
    }

    for(let i=0; i < ancestor.items.length; i++) {
      let child = ancestor.items[i];
      if (child == node) {
        ancestor.items.splice(i, 1);
        return true;
      }

      if (this._removeFromParent(child, node)) {
        return true;
      }
    }

    return false;
  }


}
