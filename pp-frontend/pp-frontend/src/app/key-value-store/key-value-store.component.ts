import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { KeyValueStoreService } from '../services/KeyValueStore';
import { checkResult } from '../utils/ObservableUtils';


@Component({
  selector: 'app-key-value-store',
  templateUrl: './key-value-store.component.html',
  styleUrls: ['./key-value-store.component.scss'],
})
export class KeyValueStoreComponent {
  busy: boolean = false;
  key0: string|undefined;
  autoCompleteKeys: string[] = ['0','1','2'];
  filteredAutoCompleteOptions: {[key: string]: string[]} = {};
  autoCompleteValues: {[Key: string]: string} = {};
  selectedStore: string|undefined;
  storeOptions: string[] = [];
  valueKeyMap: string[] = [];
  valueValueString: string = "";
  valueValueWarning: string = "";

  constructor(
      private store: KeyValueStoreService,
      private snackBar: MatSnackBar) {
        this.busy = true;
        store.getStores().subscribe(checkResult<string[]>({
          success: s => this.storeOptions = s,
          fail: e => this.showSnackError(e),
          finally: () => { this.busy = false; }
        }));
  }

  showSnackError(message?: string) {
    this.snackBar.open(`❌ ${message ?? "Error while executing the request"}`, 'Dismiss', {
      duration: 5000
    });
  }

  showSnackSuccess(message: string) {
    this.snackBar.open(`✔️ ${message}`, 'Dismiss', {
      duration: 5000
    });
  }

  autoCompleteValueChanged(key: string, value: string) {
    this.autoCompleteValues[key] = value;
    this.store.autocomplete(this.selectedStore!, value).subscribe(checkResult<string[]>({
      success: s => {
        this.filteredAutoCompleteOptions[key] = s;
      },
      fail: e => this.showSnackError(e)
    }));
  }

  _refreshAutocompleteOptions() {
    for (let key of this.autoCompleteKeys) {
      this.store.autocomplete(this.selectedStore!, this.autoCompleteValues[key] ?? "").subscribe(checkResult<string[]>({
        success: s => this.filteredAutoCompleteOptions[key] = s,
      }));
    }
  }

  _refreshValueKeyMap() {
    if (!this.autoCompleteValues['1']) {
      this.valueKeyMap = [];
      return;
    }

    this.store.getKeys(this.selectedStore!, this.autoCompleteValues['1']).subscribe(checkResult<string[]>({
      success: s => this.valueKeyMap = s,
      fail: e => this.valueKeyMap = []
    }));
  }

  selectedStoreChanged() {
    if (!this.selectedStore || this.busy) {
      return;
    }

    this._refreshValueKeyMap();

    this.busy = true;
    this.store.autocomplete(this.selectedStore, "").subscribe(checkResult<string[]>({
      success: s => {
        for(let key of this.autoCompleteKeys) {
          this.filteredAutoCompleteOptions[key] = s;
        }
      },
      fail: e => this.showSnackError(e),
      finally: () => this.busy = false
    }));
  }

  // edit keys

  addMap() {
    if (this.busy || !this.key0 || !this.autoCompleteValues['0']) {
      return;
    }

    this.busy = true;
    this.store.addKey(this.selectedStore!, this.key0, this.autoCompleteValues['0'])
      .subscribe(checkResult<null>({
        success: s => {
          this.showSnackSuccess(`Successfully added to value ${this.autoCompleteValues['0']}`);
          this._refreshAutocompleteOptions();
          this._refreshValueKeyMap();
        },
        fail: e => this.showSnackError(e),
        finally: () => this.busy = false
      }));
  }

  getKeys() {
    if (this.busy || !this.autoCompleteValues['1']) {
      this.valueKeyMap = [];
      return;
    }

    this.busy = true;
    this.store.getKeys(this.selectedStore!, this.autoCompleteValues['1']).subscribe(checkResult<string[]>({
      success: s => this.valueKeyMap = s,
      fail: e => {
        this.valueKeyMap = [];
        this.showSnackError(e);
      },
      finally: () =>this.busy = false
    }));
  }

  deleteKey(key: string) {
    if (this.busy) {
      return;
    }

    this.busy = true;
    this.store.deleteKey(this.selectedStore!, key).subscribe(checkResult<null>({
      success: s => {
        this._refreshValueKeyMap();
        this._refreshAutocompleteOptions();

      },
      fail: e => this.showSnackError(e),
      finally: () => this.busy = false
    }));
  }

  // edit values

  _clearValueValue() {
    this.valueValueWarning = "";
    this.valueValueString = "";
  }

  _isValidJson(s: string) {
    try {
      JSON.parse(s);
      return true;
    }
    catch {
      return false;
    }
  }

  getValueValue() {
    if (this.busy) {
      return;
    }

    this.busy = true;
    this._clearValueValue();

    this.store.getValueValue(this.selectedStore!, this.autoCompleteValues['2']).subscribe(checkResult<string>({
      success: s => {
        this.valueValueString = s;
        if (!this._isValidJson(s)) {
          this.showSnackSuccess("Warning: loaded value successfully, but it is not valid json.");
        }
        this.valueValueValueChanged();
      },
      fail: e => this.showSnackError(e),
      finally: () => this.busy = false
    }));
  }

  valueValueValueChanged() {
    if (this.valueValueString && !this._isValidJson(this.valueValueString)){
      this.valueValueWarning = "Warning: value is not valid json";
    } else {
      this.valueValueWarning = "";
    }
  }

  updateValueValue() {
    if (this.busy) {
      return;
    }

    this.busy = true;
    const isValidJson = this._isValidJson(this.valueValueString);
    this.store.updateValueValue(this.selectedStore!, this.autoCompleteValues['2'], this.valueValueString).subscribe(checkResult<null>({
      success: s => {
        if (!isValidJson) {
          this.showSnackSuccess("Warning: updated value successfully, but it is not valid json.");
        } else {
          this.showSnackSuccess("Update value.");
        }
      },
      fail: e => this.showSnackError(e),
      finally: () => this.busy = false
    }));
  }

  deleteValue() {
    if (this.busy) {
      return;
    }
    this.busy = true;
    const value = this.autoCompleteValues['2'];
    this.store.deleteValue(this.selectedStore!, value).subscribe(checkResult<null>({
      success: _ => {
        this.showSnackSuccess(`Deleted value ${value}.`);
        this._refreshValueKeyMap();
        this._refreshAutocompleteOptions();
      },
      fail: e => this.showSnackError(e),
      finally: () => this.busy = false
    }));
  }

  prettifyValue() {
    try {
      const s = JSON.parse(this.valueValueString);
      this.valueValueString = JSON.stringify(s, null, 4);
    } catch {
      this.showSnackError("Value is not a valid json object.");
    }
  }




}