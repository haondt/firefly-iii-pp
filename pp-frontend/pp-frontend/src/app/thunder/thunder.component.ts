import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ClientInfoDto } from '../models/dtos/ClientInfo';
import { ThunderService } from '../services/Thunder';
import { AddCaseDialog } from './add-case-dialog/add-case-dialog.component';

@Component({
  selector: 'app-thunder',
  templateUrl: './thunder.component.html',
  styleUrls: ['./thunder.component.scss'],
})
export class ThunderComponent {
    clientInfo?: ClientInfoDto;
    constructor(private thunderService: ThunderService,
        private snackBar: MatSnackBar,
        private dialog: MatDialog) {

    }

    showSnackError(error?: string) {
        this.snackBar.open(error ?? "Error while executing the request", 'Dismiss', {
            duration: 5000
        });

    }

    getStats(button: {disabled: boolean}) {
        button.disabled = true;
        this.thunderService.getClientData().subscribe(res => {
            try {
                if (res.success) {
                    this.clientInfo = res.body;
                } else {
                    this.showSnackError(res.error);
                }
            } finally {
                button.disabled = false;
            }
        })
    }

    sort(button: {disabled: boolean}) {
        button.disabled = true;
        this.thunderService.sort().subscribe(res => {
            try {
                if (!res.success) {
                    this.showSnackError(res.error);
                }
            } finally {
                button.disabled = false;
            }
        })
    }

    addCaseFromTransaction(button: { disabled: boolean }): void {
        button.disabled = true;
        this.thunderService.getFolderNames().subscribe(res => {
            try {
                if (!res.success) {
                    this.showSnackError(res.error);
                } else {
                    const dialogRef = this.dialog.open(AddCaseDialog, {
                        data: {
                            title: 'Add case from transaction',
                            folderNameOptions: res.body!
                        }
                    });
                    dialogRef.afterClosed().subscribe();
                }
            } finally {
                button.disabled = false;
            }
        })
    }
}