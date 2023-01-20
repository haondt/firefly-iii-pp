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
import { isDescendantOf, removeFromParent } from './Utils/TreeNode';
import { TransactionFieldsDialog } from './transaction-fields-dialog/transaction-fields-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { CaseModel } from '../models/Case';
import { TestRunnerService } from '../services/TestRunner';
import { ingestPostmanFile } from './Utils/PostmanFile';

@Component({
  selector: 'app-tests',
  templateUrl: './tests.component.html',
  styleUrls: ['./tests.component.scss'],
})
export class TestsComponent {
  test_id = '2';

  treeControl = new NestedTreeControl<TreeNode>(m => m.items);
  dataSource = new MatTreeNestedDataSource<TreeNode>();
  hovered: TreeNode | null = null;

  constructor(
    private testBuilder: TestBuilderService,
    private testRunner: TestRunnerService,
    private mongo: MongoDbService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog) {
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
    this.mongo.setTestData(
      this.test_id, this.testBuilder.unBuild(
        this.dataSource.data as FolderContentModel[])).subscribe();
  }

  reloadData() {
    const d = this.dataSource.data;
    this.dataSource.data = [];
    this.dataSource.data = d;
  }

  isFolderNode = (node: any) => node instanceof FolderModel;
  isTestNode = (node: any) => node instanceof TestModel;

  addChild(parent: TreeNode, child: TreeNode) {
    parent.items?.unshift(child);
    if(!this.treeControl.isExpanded(parent)){
      this.treeControl.expand(parent);
    }
  }

  addTestNode(parent: any) {
    if (this.isFolderNode(parent)) {
      this.addChild(parent, new TestModel());
      this.reloadData();
    } else {
      throw new TypeError(`Expected object of type FolderModel but received ${parent.constructor.name}`);
    }
  }

  addFolderNode(parent: any) {
    if (this.isFolderNode(parent)) {
      this.addChild(parent, new FolderModel());
      this.reloadData();
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

  checkNodeFieldAsBool = (node: {meta: {[k: string]: Object}}, fieldName: string) =>
    fieldName in node.meta && node.meta[fieldName] === true;
  toggleNodeFieldAsBool = (node: {meta: {[k: string]: Object}}, fieldName: string) =>
    node.meta[fieldName] = !this.checkNodeFieldAsBool(node, fieldName);
  isExpanded = (node: TreeNode) =>
    this.checkNodeFieldAsBool(node, 'expanded');
  toggleExpanded = (node: TreeNode) =>
    this.toggleNodeFieldAsBool(node, 'expanded');
  isChecksExpanded = (node: TestModel) =>
    this.checkNodeFieldAsBool(node, 'checks_expanded');
  toggleChecksExpanded = (node: TestModel) =>
    this.toggleNodeFieldAsBool(node, 'checks_expanded');
  isCasesExpanded = (node: TestModel) =>
    this.checkNodeFieldAsBool(node, 'cases_expanded');
  toggleCasesExpanded = (node: TestModel) =>
    this.toggleNodeFieldAsBool(node, 'cases_expanded');
  isCaseExpanded = (node: CaseModel) =>
    this.checkNodeFieldAsBool(node, 'expanded');
  toggleCaseExpanded = (node: CaseModel) =>
    this.toggleNodeFieldAsBool(node, 'expanded');

  addCheck(node: TestModel) {
    node.checks.unshift(new CheckModel());
    this.reloadData();
  }
  removeCheck(node: TestModel, check: CheckModel) {
    let i = node.checks.indexOf(check);
    if (i >= 0) {
      node.checks.splice(i, 1);
      this.reloadData();
    }
  }
  addCheckFromTransaction(node: TestModel): void {
    const dialogRef = this.dialog.open(TransactionFieldsDialog, {
      data: { title: 'Add checks from transaction' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        for (let kvp of result) {
          node.checks.unshift(new CheckModel({
            name: kvp.key
              .replace(/[^_]+/gm, (w: string) => w[0].toUpperCase() + w.slice(1).toLowerCase())
              .replace('_', ' '),
            key: kvp.key,
            value: kvp.value,
            meta: {}
          }));
        }
        this.reloadData();
      }
    });
  }

  addCase(node: TestModel) {
    node.cases.unshift(new CaseModel());
    this.reloadData();
  }
  removeCase(node: TestModel, _case: CaseModel) {
    let i = node.cases.indexOf(_case);
    if (i >= 0) {
      node.cases.splice(i, 1);
      this.reloadData();
    }
  }
  addCaseFromTransaction(node: TestModel): void {
    const dialogRef = this.dialog.open(TransactionFieldsDialog, {
      data: { title: 'Add case from transaction' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        node.cases.unshift(new CaseModel({
          body: result,
          meta: {}
        }));
        this.reloadData();
      }
    });
  }

  // drag & drop

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

    if (isDescendantOf(node, this.hovered)) {
      this.snackBar.open('The folder cannot be moved into its own subfolder.', 'Dismiss', {
        duration: 3000
      });
      return;
    }

    if (!removeFromParent(this.dataSource.data, node)){
      throw new Error(`Unable to remove node ${node.name} from its parent!`);
    }

    if (this.hovered.items === null || this.hovered.items === undefined) {
      this.hovered.items = [];
    }

    this.hovered.items.unshift(node);
    this.reloadData();
  }

  deleteNode(node: TreeNode) {
    if (!removeFromParent(this.dataSource.data, node)){
      throw new Error(`Unable to remove node ${node.name} from its parent!`);
    }
    this.reloadData();
  }

  runTests() {
    var tests = this.testRunner.prepareTests(this.dataSource.data as FolderContentModel[]);
    this.testRunner.runTests(tests).subscribe(r => {
      console.log(r);
      console.log(this.dataSource.data);
    });
  }

  handleImportPostmanFile(event: any) {
    if (event.target.files.length > 0) {
      let x = ingestPostmanFile(event.target.files[0]);
      console.log(x);
    }
  }
}

