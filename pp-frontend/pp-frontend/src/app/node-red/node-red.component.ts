import { Component } from '@angular/core';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { take } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NodeRedService } from '../services/NodeRed';


@Component({
  selector: 'app-node-red',
  templateUrl: './node-red.component.html',
  styleUrls: ['./node-red.component.scss'],
})
export class NodeRedComponent {
  busy: boolean = false;

  requestText: string | undefined;
  responseText: string = "";

  constructor(private snackBar: MatSnackBar,
    private nodeRedService: NodeRedService) {
  }

  showSnackError(error?: string) {
      this.snackBar.open(error ?? "❌ Error while executing the request", 'Dismiss', {
          duration: 5000
      });
  }

  exportFlows() {
    if (this.busy){
      return;
    }
    this.busy = true;
    this.nodeRedService.exportFlows().subscribe(res => {
      try {
        if (res.success) {
          this.snackBar.open("✔️ Successfully exported Node-Red flows", 'Dismiss', { duration: 5000 });
        } else {
          this.showSnackError(res.error);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  sendPassthrough() {
    if (this.busy
      || !this.requestText){
      return;
    }

    this.busy = true;
    this.responseText = "";
    this.nodeRedService.sendPassthrough(this.requestText).subscribe(res => {
      try {
        if (res.success) {
          this.responseText = JSON.stringify(JSON.parse(res.body!), null, 4);
        } else {
          this.showSnackError(`❌ ${res.error}`);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  formatRequest() {
    if (this.requestText) {
      try {
        this.requestText = JSON.stringify(JSON.parse(this.requestText), null, 4);
      } catch {
        this.showSnackError("❌ Invalid json");
      }
    }

  }
}