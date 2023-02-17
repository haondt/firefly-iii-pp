import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  selector: 'app-key-value-store',
  templateUrl: './key-value-store.component.html',
  styleUrls: ['./key-value-store.component.scss'],
})
export class KeyValueStoreComponent {
  busy: boolean = false;
  key0: string|undefined;
  autoCompleteOptions: {[key: string]: string[]} = {};
  autoCompleteValues: {[Key: string]: string} = {};

  constructor(
      private snackBar: MatSnackBar) {
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
    this.autoCompleteOptions[key] = ["foo", "bar", "baz"];
  }

}